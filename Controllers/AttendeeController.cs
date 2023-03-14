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
        public IActionResult RegisterAttendance(AttendeeModel attend)
        {
            try
            {
                IFirebaseClient client = new FireSharp.FirebaseClient(config);

                // check if user have registered before
                // pull all data from attendence table
                FirebaseResponse registered_response = client.Get("Attendance");
                dynamic registered_data = JsonConvert.DeserializeObject<dynamic>(registered_response.Body);
                var registered_list = new List<AttendeeModel>();
                if (registered_data != null)
                {
                    foreach (var item in registered_data)
                    {
                        registered_list.Add(JsonConvert.DeserializeObject<AttendeeModel>(((JProperty)item).Value.ToString()));
                    }
                }

                // Get selected presentation name
                var data = attend;
                
                string selectName = data.presentationName.ToString();
                string selectEmail = data.userEmail.ToString();

                // for each record
                // check if presentation name match
                // then check if email match
                // if match, break
                for (int i = 0; i < registered_list.Count; i++)
                {
                    string check_name = registered_list[i].presentationName.ToString();
                    string check_email = registered_list[i].userEmail.ToString();

                    if (check_name == selectName)
                    {
                        // check email match
                        if (check_email == selectEmail)
                        {
                            // exit
                            ModelState.AddModelError(string.Empty, "Something went wrong!!");
                            return View();
                            
                        }
                    }

                }


                // enter new record into table
                
                // combine presentation name and email
                
                data.comb_id = data.presentationName.ToString() + "_" + selectEmail;

                PushResponse response = client.Push("Attendance/", data);
                data.Id = response.Result.name;
                SetResponse setResponse = client.Set("Attendance/" + data.Id, data);

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

            client.Set("Attendance/" + review.Id, review);
            return View("Reviews");
        }

        public IActionResult Reviews()
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
            return View(list);
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
                        // accumulate score
                        totalScore = totalScore + attend_list[k].reviewScore;
                        totalCount++;

                    }
                }

                // create object
                AttendeeModel single_review = new AttendeeModel();

                if(totalCount > 0)
                {
                    // Get average score
                    average_score = totalScore / totalCount;
                }
                

                single_review.reviewScore = average_score;
                single_review.presentationName = list[i].name;


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
