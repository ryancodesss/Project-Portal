using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Project_Portal.Models
{
    public class GeneralUserModel
    {
        public string Id { get; set; } // firebase unique id 
        public string username { get; set; }
        public string fullname { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
    }
}

