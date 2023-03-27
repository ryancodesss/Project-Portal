using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace Project_Portal.Models
{
    public class RegistrationModel
    {
        public string Id { get; set; } // firebase unique id

        
        public string Email { get; set; }

        
        [DataType(DataType.Password)]
        public string Password { get; set; }

        
        [Display(Name = "Full Name")]
        public string Full_Name { get; set; }


        
        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        
        public string Affliation { get; set; }

        [DisplayName("User Type")]
        public char User_Type { get; set; } = 'S';
    }
}
