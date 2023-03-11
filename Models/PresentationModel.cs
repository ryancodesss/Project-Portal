using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;


namespace Project_Portal.Models
{
    public class PresentationModel
    {
        public string Id { get; set; } // firebase unique id 
        public string name { get; set; }
        public string description { get; set; }
        public string location { get; set; }

        [DataType(DataType.Date)]
        public string date { get; set; }

        [DataType(DataType.Time)]
        public string time { get; set; }
        public string creator { get; set; }
    }
}
