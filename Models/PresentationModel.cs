using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;


namespace Project_Portal.Models
{
    public class PresentationModel
    {
        public string Id { get; set; } // firebase unique id 
        public string presentationID { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string location { get; set; }
        public string date { get; set; }
        public string time { get; set; }
        public string creator { get; set; }
    }
}
