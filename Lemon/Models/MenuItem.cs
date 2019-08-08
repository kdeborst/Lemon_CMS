using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lemon.Models
{
    public class MenuItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Naam Gerecht")]
        public string Name { get; set; }

        [Display(Name = "Omschrijving")]
        public string Description { get; set; }

        [Display(Name = "Pittigheid")]
        public string Spiciness { get; set; }
        public enum ESpicinessScale { NA = 0, Mild = 1, Gemiddeld = 2, Hoog = 3, Extreem = 4 }

        [Display(Name = "Afbeelding")]
        public string Image { get; set; }

        [Display(Name = "Categorie")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        [Display(Name = "Subcategorie")]
        public int SubCategoryId { get; set; }

        [ForeignKey("SubCategoryId")]
        public virtual SubCategory SubCategory { get; set; }

        [Display(Name = "Prijs")]
        [Range(double.MinValue, double.MaxValue, ErrorMessage = "De prijs moet hoger dan €{0,25} zijn")]
        public double Price { get; set; }
    }
}
