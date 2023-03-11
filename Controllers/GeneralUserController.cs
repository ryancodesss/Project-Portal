﻿using FireSharp.Interfaces;
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

namespace Project_Portal.Controllers
{
    public class GeneralUserController : Controller
    {
        
        // Firebase connection
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "BJm0Xt86MfbcKsarwCPzTvT2zfOcGw72OEW5XUzq",
            BasePath = "https://portal-project-14039-default-rtdb.asia-southeast1.firebasedatabase.app"
        };

        private AuthenticationServices authService = new AuthenticationServices();


        // GET: GeneralUserController/View
        // THIS METHOD IS FOR ADMIN USER
        /**
        public ActionResult AccountDetails()
        {
            IFirebaseClient client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Users");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<GeneralUserModel>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<GeneralUserModel>(((JProperty)item).Value.ToString()));
                }
            }

            return View(list);
        }
        **/

        // GET: GeneralUserController/View account
        public async Task<IActionResult> AccountDetailsAsync()
        {
            string token = HttpContext.Session.GetString("_UserToken");
            string email = await authService.GetUserEmailByToken(token);
            string pass = HttpContext.Session.GetString("_UserPassword");
            ViewBag.Email = email;
            ViewBag.Password = pass;
            return View();
        }


        // POST: GeneralUserController/View account
        // this methods will upload new records to database
        
        // Require to validate if there is existing records
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AccountDetails(GeneralUserModel generalUser)
        {
            try
            {
                IFirebaseClient client = new FireSharp.FirebaseClient(config);
                var data = generalUser;
                PushResponse response = client.Push("Users/", data);
                data.Id = response.Result.name;
                SetResponse setResponse = client.Set("Users/" + data.Id, data);

                if (setResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ModelState.AddModelError(string.Empty, "Added Succesfully");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Something went wrong!!");
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View("Index");
        }
        

        /**
        // GET: GeneralUserController/Edit/5
        public ActionResult EditAccountDetails(string id)
        {
            IFirebaseClient client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Users/" + id);
            GeneralUserModel data = JsonConvert.DeserializeObject<GeneralUserModel>(response.Body);
            return View(data);
        }
        
        
        // POST: GeneralUserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAccountDetails(GeneralUserModel user)
        {
            try
            {
                IFirebaseClient client = new FireSharp.FirebaseClient(config);
                SetResponse response = client.Set("Users/" + user.Id, user);
                return RedirectToAction("AccountDetails");
            }
            catch
            {
                return View();
            }
        }

        
        // POST: GeneralUserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            try
            {
                IFirebaseClient client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Delete("User/" + id);
                return RedirectToAction("AccountDetails");

            }
            catch
            {
                return View();
            }
        }
        **/
    }
}
