using Microsoft.AspNetCore.Mvc;
using Project_Portal.Models;
using System.Diagnostics;
using Firebase.Auth;
using Newtonsoft.Json;
using Project_Portal.Services;
using FireSharp.Exceptions;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json.Linq;
using NuGet.Common;
using System.Net;

namespace Project_Portal.Controllers
{
    public class HomeController : Controller
    {
        //Initialize Sevices namespace
        private DatabaseServices dbService = new DatabaseServices();
        private AuthenticationServices authService = new AuthenticationServices();

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

        // View method for admin
        public IActionResult IndexAdmin()
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
                string uid = fbAuthLink.User.LocalId.ToString();

                //Set current user's id to be uid
                //registrationModel.Id = uid;

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
                    HttpContext.Session.SetString("_UserEmail", registrationModel.Email);

                    return View("IndexGeneral");
                }
            }
            catch (FirebaseAuthException ex)
            {
                ModelState.AddModelError(String.Empty, ex.InnerException.ToString());
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
                    // filter between staff, admin and general user
                    // add code here

                    HttpContext.Session.SetString("_UserToken", token);
                    HttpContext.Session.SetString("_UserEmail", loginModel.Email);

                    // Check if userType is staff or student
                    // Get user uid by token

                    // this uid is authentication uid
                    var uid = await authService.GetCurrentUser(token);
                    
                    if (uid != null)
                    {
                        // Get user type by uid
                        char userType = await dbService.GetUserTypeByEmail(loginModel.Email);
                        //Set usertype in session
                        //HttpContext.Session.SetString ("_UserType", userType);


                        if (userType == 'S')
                        {
                            return RedirectToAction("IndexGeneral");
                        }
                        else if(userType == 'T')
                        {
                            return RedirectToAction("IndexStaff");
                        }
                        else if (userType == 'A')
                        {
                            return RedirectToAction("IndexAdmin");
                        }

                        return RedirectToAction("IndexGeneral");
                    }
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
            HttpContext.Session.Clear();
            return RedirectToAction("SignIn");
        }


        // GET: view all acounts
        public async Task<IActionResult> ViewAccountsAsync()
        {
            var list = await dbService.GetAllUser();

            return View(list);
        }

        // GET admin add new staff form
        public IActionResult AddStaff()
        {
            return View();
        }

        // Validate and process the registration request submitted by user
        [HttpPost]
        public async Task<IActionResult> AddStaffAsync(RegistrationModel registrationModel)
        {
            try
            {
                //create the user
                var fbAuthLink = await auth.CreateUserWithEmailAndPasswordAsync(registrationModel.Email, registrationModel.Password);

                //Get current user's authentication ID
                var uid = fbAuthLink.User.LocalId.ToString();
                //Set current user's id to be uid
                registrationModel.Id = uid;

                registrationModel.User_Type = 'T';

                try
                {
                    dbService.AddUser(registrationModel);
                }
                catch (FirebaseException ex)
                {
                    var firebaseEx = JsonConvert.DeserializeObject<FirebaseException>(ex.Message);
                    //ModelState.AddModelError(String.Empty, firebaseEx.Message);
                    ModelState.AddModelError(string.Empty, "Something went wrong!!");
                    return View(registrationModel);
                }
            
                ModelState.AddModelError(string.Empty, "Added Succesfully");
                
                
            }
            catch (FirebaseAuthException ex)
            {
                ModelState.AddModelError(String.Empty, ex.InnerException.ToString());
                return View(registrationModel);
            }

            return View();

        }

        public ActionResult DeleteUser(string id)
        {
            try
            {
                IFirebaseConfig config = new FireSharp.Config.FirebaseConfig
                {
                    AuthSecret = "BJm0Xt86MfbcKsarwCPzTvT2zfOcGw72OEW5XUzq",
                    BasePath = "https://portal-project-14039-default-rtdb.asia-southeast1.firebasedatabase.app"
                };

                IFirebaseClient client = new FireSharp.FirebaseClient(config);
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Delete("Users/" + id);

                // retrieve authentication table uid
                //var uid = await authService.GetCurrentUser(token);
                
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
                


            return View("IndexAdmin");

        }

        // GET: Admin edit user information
        // prefill edit form with selected user information
        public ActionResult EditUser(string id)
        {
            IFirebaseConfig config = new FireSharp.Config.FirebaseConfig
            {
                AuthSecret = "BJm0Xt86MfbcKsarwCPzTvT2zfOcGw72OEW5XUzq",
                BasePath = "https://portal-project-14039-default-rtdb.asia-southeast1.firebasedatabase.app"
            };

            IFirebaseClient client = new FireSharp.FirebaseClient(config);
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Users/" + id);
            RegistrationModel data = JsonConvert.DeserializeObject<RegistrationModel>(response.Body);
            
            // populate viewbag
            ViewBag.Full_Name = data.Full_Name;
            ViewBag.Phone = data.Phone;
            ViewBag.Affliation = data.Affliation;
            ViewBag.User_Type = data.User_Type;
            ViewBag.Email = data.Email;
            ViewBag.Password = data.Password;

            return View();
        }

        // POST: Admin edit user information
        // upload edited information
        [HttpPost]
        public ActionResult EditUser(RegistrationModel account)
        {
            try
            {
                IFirebaseConfig config = new FireSharp.Config.FirebaseConfig
                {
                    AuthSecret = "BJm0Xt86MfbcKsarwCPzTvT2zfOcGw72OEW5XUzq",
                    BasePath = "https://portal-project-14039-default-rtdb.asia-southeast1.firebasedatabase.app"
                };

                IFirebaseClient client = new FireSharp.FirebaseClient(config);
                client = new FireSharp.FirebaseClient(config);

                SetResponse response = client.Set("Users/" + account.Id, account);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
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