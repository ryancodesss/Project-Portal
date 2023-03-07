using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Project_Portal.Models
{
    public class AttendeeModel
    {
        public string Id { get; set; } // firebase unique id 
        public string presentationID { get; set; }

        public string userID { get; set; }

        public string reviewDesc { get; set; }
        public string reviewScore { get; set; }
    }
}
