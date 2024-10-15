using System.ComponentModel.DataAnnotations;

namespace BitirmeProjesi.Models
{

    public class BrandModel
    {
        [Key]
        [Display(Name = "Model Id")]
        public int ModelId { get; set; }

        [Display(Name = "Araç Markası")]
        [Required(ErrorMessage = "Araç Markası Eksik")]
        public int BrandId { get; set; }

        [Display(Name = "Araç Modeli")]
        [Required(ErrorMessage = "Araç Modeli Eksik")]
        [StringLength(50)]
        public string? ModelName { get; set; }

        [Display(Name = "Araç Kategorisi")]
        [Required(ErrorMessage = "Araç Kategorisi Eksik")]
        public int CategoryId { get; set; }  
    }
}