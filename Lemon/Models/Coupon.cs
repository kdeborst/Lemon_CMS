using System.ComponentModel.DataAnnotations;

namespace Lemon.Models
{
    public class Coupon
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Coupon Naam")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Coupon Type")]
        public string CouponType { get; set; }

        public enum ECouponType { Procent = 0, Euro = 1 }

        [Required]
        [Display(Name = "Korting")]
        public double Discount { get; set; }

        [Required]
        [Display(Name = "Minimumbedrag")]
        public double MinimumAmount { get; set; }

        [Display(Name = "Afbeelding")]
        public byte[] Picture { get; set; }

        [Display(Name = "Is Actief")]
        public bool IsActive { get; set; }

    }
}
