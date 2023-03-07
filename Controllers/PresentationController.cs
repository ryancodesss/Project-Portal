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
    public class PresentationController : Controller
    {

        // Firebase connection
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "BJm0Xt86MfbcKsarwCPzTvT2zfOcGw72OEW5XUzq",
            BasePath = "https://portal-project-14039-default-rtdb.asia-southeast1.firebasedatabase.app"
        };

        // GET: Create presentation event
        public IActionResult CreatePresentation()
        {

            return View();
            
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

        // GET: Edit presentation details
        public IActionResult EditPresentation()
        {
            return View();
        }

        
        // POST: Create presentation event
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreatePresentation(PresentationModel presentation)
        {
            
            try
            {
                IFirebaseClient client = new FireSharp.FirebaseClient(config);
                var data = presentation;
                PushResponse response = client.Push("Presentation/", data);
                data.Id = response.Result.name;
                SetResponse setResponse = client.Set("Presentation/" + data.Id, data);

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

        // POST: Delete presentation event button
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePresentation(string id)
        {
            try
            {
                IFirebaseClient client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Delete("Presentation/" + id);
                return RedirectToAction("StaffViewPresentation");

            }
            catch
            {
                return View();
            }
        }

    }
}
