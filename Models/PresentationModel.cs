using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace Project_Portal.Models
{
    public class PresentationModel
    {
        public string Id { get; set; } // firebase unique id

        [Required]
        [DisplayName("Name")]
        public string name { get; set; }

        [Required]
        [DisplayName("Description")]
        public string description { get; set; }

        [Required]
        [DisplayName("Location")]
        public string location { get; set; }

        [Required]
        [DisplayName("Date")]
        [DataType(DataType.Date)]
        public string date { get; set; }

        [Required]
        [DisplayName("Time")]
        [DataType(DataType.Time)]
        public string time { get; set; }

        [DisplayName("Creator")]
        public string creator { get; set; }
    }
}
