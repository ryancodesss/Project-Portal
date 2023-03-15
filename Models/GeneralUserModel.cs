using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Project_Portal.Models
{
    public class GeneralUserModel
    {
        public string Id { get; set; } // firebase unique id 
        public string Email { get; set; } // user's email address

        public string Password { get; set; } // user's login password
        public string Fullname { get; set; } // user's name
        public string Phone { get; set; } // user's phone number
        public string Affliation { get; set; } // user's school affliation
        public char User_Type { get; set; } // user's type
    }
}

