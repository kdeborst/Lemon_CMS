using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Lemon.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [Display(Name = "Naam")]
        public string Name { get; set; }

        [Display(Name = "Adres")]
        public string Address { get; set; }

        [Display(Name = "Postcode")]
        public string Postcode { get; set; }

        [Display(Name = "Stad")]
        public string City { get; set; }

    }
}
