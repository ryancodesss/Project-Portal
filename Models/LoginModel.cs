using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Project_Portal.Models
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } // user's email
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } // user's password
    }

    
}
