using System.ComponentModel.DataAnnotations;

namespace Lemon.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Naam Categorie")]
        public string Name { get; set; }
    }
}
