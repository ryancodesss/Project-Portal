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


        // GET User register attendence
        public IActionResult RegisterAttendance(string primarykey)  
        {
            string email = HttpContext.Session.GetString("_UserEmail");
            
            ViewBag.userEmail = email;
            ViewBag.presentationName = primarykey;
            return View();
        }

        // EHEREASNDOIASNOIANFAF
        //ASDASDASDASDASDD
        // POST user registration attendence
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegisterAttendance(AttendeeModel attend)
        {
            try
            {
                IFirebaseClient client = new FireSharp.FirebaseClient(config);
                var data = attend;
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
        

    }
}
