using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Project_Portal.Models
{
    public class AttendeeModel
    {
        public string Id { get; set; }
        public string comb_id { get; set; } // combination of presentation name and user email

        [DisplayName("Presentation Name")]
        public string presentationName { get; set; }

        [DisplayName("User Email")]
        public string userEmail { get; set; }

        [DisplayName("Review Comment")]
        public string reviewDesc { get; set; }

        [DisplayName("Review Score")]
        [DefaultValue(null)]
        public string reviewScore { get; set; }
    }
}
