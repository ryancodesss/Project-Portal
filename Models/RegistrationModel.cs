using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Project_Portal.Models
{
    public class RegistrationModel
    {
        public string Id { get; set; } // firebase unique id

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string Full_Name { get; set; }


        [Required]
        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Required]
        public string Affliation { get; set; }

        [Required]
        public char User_Type { get; set; } = 'S';
    }
}
