using FireSharp.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FireSharp.Config;
using FireSharp.Interfaces;
using Project_Portal.Models;
using FireSharp.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using FireSharp;
using System.Linq.Expressions;
using Project_Portal.Services;
using Microsoft.AspNetCore.Razor.Language.Extensions;

namespace Project_Portal.Controllers
{
    public class AttendeeController : Controller
    {

        // Firebase connection
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "BJm0Xt86MfbcKsarwCPzTvT2zfOcGw72OEW5XUzq",
            BasePath = "https://portal-project-14039-default-rtdb.asia-southeast1.firebasedatabase.app"
        };

        private DatabaseServices dbService = new DatabaseServices();


        private AuthenticationServices authService = new AuthenticationServices();

        // GET User register attendence
        public async Task<IActionResult> RegisterAttendanceAsync(string primarykey)  
        {
            string token = HttpContext.Session.GetString("_UserToken");
            string email = await authService.GetUserEmailByToken(token);

            ViewBag.userEmail = email;
            ViewBag.presentationName = primarykey;
            return View();
        }


        // POST user registration attendence
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAttendanceAsync(AttendeeModel attend)
        {
            try
            {
                // check if user have registered before
                // pull all data from attendence table
                var attendanceList = await dbService.GetAllAttendee();

                //Get all completed presentation
                var completedPresentationList = await dbService.GetAllCompletedPresentation();

                //Current presentionId of the presentation user is registering + user email
                attend.comb_id = attend.presentationName + "_" + attend.userEmail;

                // for each record
                // check if presentationId match
                // then check if user email match
                // if match, break
                foreach(var attendance in attendanceList)
                {
                    if(attendance.comb_id == attend.comb_id)
                    {
                        // exit
                        ModelState.AddModelError(string.Empty, "You already registered for this presentation");
                        return View();
                    }
                }

                // enter new record into table
                SetResponse setResponse = await dbService.AddAttendee(attend);

                if (setResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ModelState.AddModelError(string.Empty, "Added Succesfully");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Something went wrong!!");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View();
        }
        
        
        // GET review form
        public async Task<IActionResult> ReviewForm(string reviewName)
        {
           
            // prefill textbox with presentation name
            ViewBag.presentName = reviewName;

            return View();
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReviewForm(AttendeeModel review)
        {

            // Get presentation id
            IFirebaseClient client = new FireSharp.FirebaseClient(config);
            FirebaseResponse list_response = client.Get("Attendance");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(list_response.Body);
            var list = new List<AttendeeModel>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<AttendeeModel>(((JProperty)item).Value.ToString()));
                }
            }

            for(int i = 0; i < list.Count; i++)
            {
                if (list[i].presentationName == review.presentationName)
                {
                    review.Id = list[i].Id;
                    break;
                }
            }

            // create presentation uid
            string token = HttpContext.Session.GetString("_UserToken");
            string email = await authService.GetUserEmailByToken(token);


            // set all variables for review object
            review.comb_id = review.presentationName + "_" + email;
            review.userEmail = email;

            SetResponse setResponse = client.Set("Attendance/" + review.Id, review);

            if (setResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                ModelState.AddModelError(string.Empty, "Added Succesfully");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Something went wrong!!");
            }

            return View();
        }

        // GET: user only able to view all of their own reviews
        public async Task<IActionResult> Reviews()
        {
            // Get presentation id
            IFirebaseClient client = new FireSharp.FirebaseClient(config);
            FirebaseResponse list_response = client.Get("Attendance");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(list_response.Body);
            var list = new List<AttendeeModel>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<AttendeeModel>(((JProperty)item).Value.ToString()));
                }
            }

            var attend_list = new List<AttendeeModel>();

            // get user email
            string token = HttpContext.Session.GetString("_UserToken");
            string email = await authService.GetUserEmailByToken(token);

            for (int i = 0; i < list.Count; i ++)
            {
                if(list[i].userEmail == email)
                {
                    attend_list.Add(list[i]);
                }
            }

            return View(attend_list);
        }

        // GET: Staff view all presentations and their average score
        public IActionResult StaffReviews()
        {
            // Pull presentation list
            IFirebaseClient client = new FireSharp.FirebaseClient(config);
            FirebaseResponse list_response = client.Get("Presentation");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(list_response.Body);
            var list = new List<PresentationModel>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<PresentationModel>(((JProperty)item).Value.ToString()));
                }
            }


            // Pull attendance list
            FirebaseResponse attend_response = client.Get("Attendance");
            dynamic attend_data = JsonConvert.DeserializeObject<dynamic>(attend_response.Body);
            var attend_list = new List<AttendeeModel>();
            if (attend_data != null)
            {
                foreach (var item in attend_data)
                {
                    attend_list.Add(JsonConvert.DeserializeObject<AttendeeModel>(((JProperty)item).Value.ToString()));
                }
            }

            var accumulate_list = new List<AttendeeModel>();

            for (int i = 0; i < list.Count; i++)
            {
                int totalScore = 0;
                int totalCount = 0;
                int average_score = 0;

                for (int k = 0; k < attend_list.Count; k++)
                {
                    if (attend_list[k].presentationName == list[i].name)
                    {
                        if (Convert.ToInt32(attend_list[k].reviewScore) > 0)
                        {
                            // accumulate score
                            totalScore = totalScore + Convert.ToInt32(attend_list[k].reviewScore);
                            totalCount++;
                        }
                    }
                }

                // create object
                AttendeeModel single_review = new AttendeeModel();

                if(totalCount > 0)
                {
                    // Get average score
                    average_score = totalScore / totalCount;
                    single_review.reviewScore = average_score.ToString();
                    single_review.presentationName = list[i].name;
                }
                else
                {
                    single_review.reviewScore = "No review recieved";
                    single_review.presentationName = list[i].name;
                }
                

                // Store average and name in list
                accumulate_list.Add(single_review);
                //accumulate_list[i].reviewScore = average_score;
                //accumulate_list[i].presentationName = list[i].name;
            }


            return View(accumulate_list);
        }

        // GET all reviews for selected presentation for staff viewing
        public IActionResult StaffPresentReviews(string primarykey)
        {
            // Pull attendance list
            IFirebaseClient client = new FireSharp.FirebaseClient(config);
            FirebaseResponse attend_response = client.Get("Attendance");
            dynamic attend_data = JsonConvert.DeserializeObject<dynamic>(attend_response.Body);
            var attend_list = new List<AttendeeModel>();
            if (attend_data != null)
            {
                foreach (var item in attend_data)
                {
                    attend_list.Add(JsonConvert.DeserializeObject<AttendeeModel>(((JProperty)item).Value.ToString()));
                }
            }

            var list = new List<AttendeeModel>();
            for(int i = 0; i < attend_list.Count; i++)
            {
                if (attend_list[i].presentationName == primarykey)
                {
                    list.Add(attend_list[i]);
                }
            }


            return View(list);
        }
    }
}
