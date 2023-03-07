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
        public IActionResult ViewAllPresentations()
        {
            return View();
        }

        public IActionResult RegisterAttendance()  
        {
            return View();
        }
        public IActionResult ReviewPresentation()
        {
            return View();
        }
    }
}
