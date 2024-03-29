﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lemon.Models
{
    public class SubCategory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Naam Subcategorie")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Categorie")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

    }
}
