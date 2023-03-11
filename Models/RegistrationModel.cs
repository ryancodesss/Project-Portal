using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Project_Portal.Models
{
    public class RegistrationModel
    {
        public string Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string First_Name { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string Last_Name { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string Phone_Num { get; set; }

        [Required]
        public string Affliation { get; set; }

        [Required]
        public char User_Type { get; set; } = 'S';
    }
}
