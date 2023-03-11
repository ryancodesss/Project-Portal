using Microsoft.AspNetCore.Mvc;
using Project_Portal.Models;
using System.Diagnostics;
using Firebase.Auth;
using Newtonsoft.Json;
using Project_Portal.Services;
using FireSharp.Exceptions;

namespace Project_Portal.Controllers
{
    public class HomeController : Controller
    {
        //Initialize Sevices namespace
        public DatabaseServices dbService = new DatabaseServices();

        // Declare firebase authentication object
        FirebaseAuthProvider auth;

        // Firebase web API link
        public HomeController()
        {
            auth = new FirebaseAuthProvider(
                            new FirebaseConfig("AIzaSyC1ZXHDjcnOy5lC24mCQcyaKPRFih7KP5Q"));
        }

        // Main page after general user login
        public IActionResult IndexGeneral()
        {
            var token = HttpContext.Session.GetString("_UserToken");

            if (token != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("SignIn");
            }
        }

        // Main page after staff user login
        public IActionResult IndexStaff()
        {
            var token = HttpContext.Session.GetString("_UserToken");

            if (token != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("SignIn");
            }
        }

        // View method for privacy page
        public IActionResult Privacy()
        {
            return View();
        }

        // View method for registration page
        public IActionResult Registration()
        {
            return View();
        }

        // View method for sign in page
        public IActionResult SignIn()
        {
            return View();
        }


        // Handle error when changing view
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Validate and process the registration request submitted by user
        [HttpPost]
        public async Task<IActionResult> Registration(RegistrationModel registrationModel)
        {
            try
            {
                //create the user
                await auth.CreateUserWithEmailAndPasswordAsync(registrationModel.Email, registrationModel.Password);
                //log in the new user
                var fbAuthLink = await auth
                                .SignInWithEmailAndPasswordAsync(registrationModel.Email, registrationModel.Password);
                string token = fbAuthLink.FirebaseToken;
                HttpContext.Session.SetString("_UserToken", token);

                //Get current user's authentication ID
                var uid = fbAuthLink.User.LocalId.ToString();
                //Set current user's id to be uid
                registrationModel.Id = uid;

                try
                {
                    dbService.AddUser(registrationModel);
                }
                catch(FirebaseException ex)
                {
                    var firebaseEx = JsonConvert.DeserializeObject<FirebaseException>(ex.Message);
                    ModelState.AddModelError(String.Empty, firebaseEx.Message);
                    return View(registrationModel);
                }

                //saving the token in a session variable
                if (token != null)
                {
                    HttpContext.Session.SetString("_UserToken", token);

                    return View("IndexGeneral");
                }
            }
            catch (FirebaseAuthException ex)
            {
                var firebaseEx = JsonConvert.DeserializeObject<FirebaseError>(ex.Message);
                ModelState.AddModelError(String.Empty, firebaseEx.error.message);
                return View(registrationModel);
            }

            return View();

        }

        // Validate and process the sign in request submitted by user
        [HttpPost]
        public async Task<IActionResult> SignIn(LoginModel loginModel)
        {
            try
            {
                //log in an existing user
                var fbAuthLink = await auth
                                .SignInWithEmailAndPasswordAsync(loginModel.Email, loginModel.Password);
                string token = fbAuthLink.FirebaseToken;
                //save the token to a session variable
                if (token != null)
                {
                    HttpContext.Session.SetString("_UserToken", token);

                    // Check if userType is staff or student
                    // INSERT ALGO HERE
                    return RedirectToAction("IndexGeneral");
                }

            }
            catch (FirebaseAuthException ex)
            {
                var firebaseEx = JsonConvert.DeserializeObject<FirebaseError>(ex.ResponseData);
                ModelState.AddModelError(String.Empty, firebaseEx.error.message);
                return View(loginModel);
            }

            return View();
        }

        // Sign-out process
        public IActionResult LogOut()
        {
            HttpContext.Session.Remove("_UserToken");
            return RedirectToAction("SignIn");
        }

    }
}