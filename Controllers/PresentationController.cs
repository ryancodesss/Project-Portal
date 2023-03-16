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
using Firebase.Auth;
using FireSharp.Exceptions;
using System.Net;
using System.Collections;
using System.Collections.Generic;

namespace Project_Portal.Controllers
{
    public class PresentationController : Controller
    {

        IFirebaseConfig config = new FireSharp.Config.FirebaseConfig
        {
            AuthSecret = "BJm0Xt86MfbcKsarwCPzTvT2zfOcGw72OEW5XUzq",
            BasePath = "https://portal-project-14039-default-rtdb.asia-southeast1.firebasedatabase.app"
        };

        //Initialize Sevices namespace
        private DatabaseServices dbService = new DatabaseServices();
        private AuthenticationServices authService = new AuthenticationServices();


        // GET: Create presentation event
        public async Task<IActionResult> StaffCreatePresentAsync()
        {
            // retrieve user id from session
            string creator = await authService.GetUserEmailByToken(HttpContext.Session.GetString("_UserToken"));
            // prefill form textbox values
            ViewBag.creator = creator;

            return View();
            
        }

        // GET: View Up-coming Presentations
        public async Task<IActionResult> GeneralViewPresentAsync()
        {
            //Get all upcoming presentation based on datetime
            //Presentation must have a later datetime than current datetime
            var list = await dbService.GetAllUpcomingPresentation();

            return View(list);
        }

        // GET: View Completed Presentations
        public async Task<IActionResult> GeneralViewCompletedPresentAsync()
        {
            //Get all upcoming presentation based on datetime
            //Presentation must have a later datetime than current datetime
            var list = await dbService.GetAllCompletedPresentation();

            return View(list);
        }

        // GET: View all presentation events
        public IActionResult StaffViewPresentation()
        {
            IFirebaseClient client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Presentation");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<PresentationModel>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<PresentationModel>(((JProperty)item).Value.ToString()));
                }
            }

            return View(list);
        }


        // GET: View registered presentations
        public IActionResult GeneralAttendPresent()
        {
            IFirebaseClient client = new FireSharp.FirebaseClient(config);

            string userEmail = HttpContext.Session.GetString("_UserEmail");
            FirebaseResponse response = client.Get("Attendance/" + userEmail);

            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<PresentationModel>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<PresentationModel>(((JProperty)item).Value.ToString()));
                }
            }

            return View(list);
        }

        // GET: Edit presentation details
        public async Task<IActionResult> EditPresentation(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var presentation = await dbService.GetPresentationById(id);
            if (presentation == null)
            {
                return NotFound();
            }
            return View(presentation);
        }


        // POST: Create presentation event
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StaffCreatePresentAsync(PresentationModel presentation)
        {
            try
            {
                // check if presentation name exist in records
                IFirebaseClient client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Presentation");
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                var list = new List<PresentationModel>();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        list.Add(JsonConvert.DeserializeObject<PresentationModel>(((JProperty)item).Value.ToString()));
                    }
                }

                // if name matches, return view and show error message
                for(int i = 0; i < list.Count; i++)
                {
                    if (list[i].name == presentation.name)
                    {
                        ModelState.AddModelError(string.Empty, "Something went wrong!!");
                        return View();
                    }
                }

                //Get current user token
                var token = HttpContext.Session.GetString("_UserToken");
                //Get current user id based on token
                string uid = await authService.GetCurrentUser(token);
                string email = await authService.GetUserEmailByToken(token);
                //Set presentation Model's creator to uid
                presentation.creator = email;
                //Add presentation
                SetResponse setResponse = await dbService.AddPresentation(presentation);

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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPresentation(string id, [Bind("Id,presentationID,name,description,location,date,time,creator")] PresentationModel presentation)
        {
            if (id != presentation.Id)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                try
                {
                    //Update DB
                    HttpStatusCode statusCode = await dbService.EditPresentationById(id, presentation);
                }
                catch (FirebaseException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                return RedirectToAction(nameof(StaffViewPresentation));
            }
            return View(presentation);
        }


        // GET registered presentations table
        public async Task<IActionResult> RegisteredPresentation()
        {
            //Get attendance table
            //Get User email
            //Get completed presentation table
            //

            // retrieve records from attendance table
            IFirebaseClient client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Attendance");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<AttendeeModel>();

            if (data != null)
            {
                foreach (var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<AttendeeModel>(((JProperty)item).Value.ToString()));
                }
            }

            // filter list for user email
            // retrieve email from session
            string token = HttpContext.Session.GetString("_UserToken");
            string email = await authService.GetUserEmailByToken(token);

            var present_list = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].userEmail == email)
                {
                    present_list.Add(list[i].presentationName.ToString());
                }
            }

            // retrieve records from presentation table
            FirebaseResponse present_response = client.Get("Presentation");
            dynamic present_data = JsonConvert.DeserializeObject<dynamic>(present_response.Body);
            var all_present_list = new List<PresentationModel>();
            var registered_present_list = new List<PresentationModel>();

            if (present_data != null)
            {
                foreach (var item in present_data)
                {
                    all_present_list.Add(JsonConvert.DeserializeObject<PresentationModel>(((JProperty)item).Value.ToString()));
                }
            }

            // filter list using user email
            for (int i = 0; i < all_present_list.Count; i++)
            {
                for(int j = 0; j < present_list.Count; j++)
                {
                    if (all_present_list[i].name.ToString() == present_list[j].ToString())
                    {
                        registered_present_list.Add(all_present_list[i]);
                    }
                }
                
            }

            return View(registered_present_list);
        }

        // GET: Staff view their own presentations
        public async Task<IActionResult> CreatedPresentations()
        {
            IFirebaseClient client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Presentation");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<PresentationModel>();

            if (data != null)
            {
                foreach (var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<PresentationModel>(((JProperty)item).Value.ToString()));
                }
            }

            // filter list for user email
            // retrieve email from session
            string token = HttpContext.Session.GetString("_UserToken");
            string email = await authService.GetUserEmailByToken(token);

            var present_list = new List<PresentationModel>();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].creator == email)
                {
                    present_list.Add(list[i]);
                }
            }


            return View(present_list);  
        }

        // GET: admin view presentations
        public IActionResult AdminPresentView()
        {
            IFirebaseClient client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Presentation/");

            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<PresentationModel>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<PresentationModel>(((JProperty)item).Value.ToString()));
                }
            }

            return View(list);

        }

        // POST: Delete presentation event button
        // delete all presentation records in attendance table
        // this method is for staff
        public ActionResult DeletePresentation(string id)
        {
            try
            {
                IFirebaseClient client = new FireSharp.FirebaseClient(config);
                FirebaseResponse single_response = client.Get("Presentation/");
                dynamic get_data = JsonConvert.DeserializeObject<dynamic>(single_response.Body);
                var get_list = new List<PresentationModel>();
                if (get_data != null)
                {
                    foreach (var item in get_data)
                    {
                        get_list.Add(JsonConvert.DeserializeObject<PresentationModel>(((JProperty)item).Value.ToString()));
                    }
                }

                string selected_name = "";
                foreach (var item in get_list)
                {
                    if(item.Id == id)
                    {
                        selected_name = item.name;
                    }
                }

                FirebaseResponse response = client.Delete("Presentation/" + id);

                // delete all presentation records in attendance table
                FirebaseResponse get_response = client.Get("Attendance/");

                dynamic attend_data = JsonConvert.DeserializeObject<dynamic>(get_response.Body);
                var attend_list = new List<AttendeeModel>();
                if (attend_data != null)
                {
                    foreach (var item in attend_data)
                    {
                        attend_list.Add(JsonConvert.DeserializeObject<AttendeeModel>(((JProperty)item).Value.ToString()));
                    }
                }

                for(int i = 0; i < attend_list.Count; i++)
                {
                    if (attend_list[i].presentationName == selected_name)
                    {
                        FirebaseResponse delete_response = client.Delete("Attendance/" + attend_list[i].Id);
                    }
                }


                return View();

            }
            catch
            {
                return View();
            }
        }

        // POST: Delete presentation event button
        // delete all presentation records in attendance table
        // this method is for admin
        public ActionResult AdminDeletePresentation(string id)
        {
            try
            {
                IFirebaseClient client = new FireSharp.FirebaseClient(config);
                FirebaseResponse single_response = client.Get("Presentation/");
                dynamic get_data = JsonConvert.DeserializeObject<dynamic>(single_response.Body);
                var get_list = new List<PresentationModel>();
                if (get_data != null)
                {
                    foreach (var item in get_data)
                    {
                        get_list.Add(JsonConvert.DeserializeObject<PresentationModel>(((JProperty)item).Value.ToString()));
                    }
                }

                string selected_name = "";
                foreach (var item in get_list)
                {
                    if (item.Id == id)
                    {
                        selected_name = item.name;
                    }
                }

                FirebaseResponse response = client.Delete("Presentation/" + id);

                // delete all presentation records in attendance table
                FirebaseResponse get_response = client.Get("Attendance/");

                dynamic attend_data = JsonConvert.DeserializeObject<dynamic>(get_response.Body);
                var attend_list = new List<AttendeeModel>();
                if (attend_data != null)
                {
                    foreach (var item in attend_data)
                    {
                        attend_list.Add(JsonConvert.DeserializeObject<AttendeeModel>(((JProperty)item).Value.ToString()));
                    }
                }

                for (int i = 0; i < attend_list.Count; i++)
                {
                    if (attend_list[i].presentationName == selected_name)
                    {
                        FirebaseResponse delete_response = client.Delete("Attendance/" + attend_list[i].Id);
                    }
                }


                return View("AdminPresentView");

            }
            catch
            {
                return View();
            }
        }

    }
}
