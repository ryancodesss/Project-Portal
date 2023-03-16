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
        public string presentationId { get; set; }
        public string presentationName { get; set; }

        public string userEmail { get; set; }

        public string reviewDesc { get; set; }

        [DefaultValue(null)]
        public string reviewScore { get; set; }
    }
}
