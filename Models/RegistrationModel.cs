using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace Project_Portal.Models
{
    public class RegistrationModel
    {
        public string Id { get; set; } // firebase unique id

        
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
        [DisplayName("Affiliation")]
        public string Affliation { get; set; }

        [DisplayName("User Type")]
        public char User_Type { get; set; } = 'S';

        public List<SelectListItem> User_Types { get; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "S", Text = "Public" },
            new SelectListItem { Value = "T", Text = "Staff" },
            new SelectListItem { Value = "A", Text = "Admin"  },
        };
    }
}
