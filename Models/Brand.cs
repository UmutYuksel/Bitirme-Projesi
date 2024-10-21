using System.ComponentModel.DataAnnotations;

namespace BitirmeProjesi.Models
{

    public class Brand
    {

        [Display(Name = "Marka ID")]
        public int BrandId { get; set; }

        [Display(Name = "Araç Markası")]
        [Required(ErrorMessage = "Araç Markası Eksik")]
        [StringLength(50)]
        public string? BrandName { get; set; }
        public int CategoryId { get; set; } 
        public Category? Category { get; set; } 

    }
}