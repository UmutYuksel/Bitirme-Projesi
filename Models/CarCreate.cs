using System.ComponentModel.DataAnnotations;

namespace BitirmeProjesi.Models
{
    public class CarCreate
    {
        [Key]
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

        [Display(Name = "Araç Yılı")]
        [Required(ErrorMessage = "Araç Yılı Eksik")]
        public int Year { get; set; }

        [Display(Name = "Kilometre")]
        [Required(ErrorMessage = "Kilometre Eksik")]
        public int Millage { get; set; }

        [Display(Name = "Beygir Gücü")]
        [Required(ErrorMessage = "Beygir Gücü Eksik")]
        public int HorsePower { get; set; }

        [Display(Name = "Maksimum Tork")]
        [Required(ErrorMessage = "Maksimum Tork Eksik")]
        public int MaxTorque { get; set; }

        [Display(Name = "Araç Rengi")]
        [Required(ErrorMessage = "Araç Rengi Eksik")]
        public string? Color { get; set; }

        [Display(Name = "Şanzıman Tipi")]
        [Required(ErrorMessage = "Şanzıman Tipi Eksik")]
        public TransmissionType transmissionType { get; set; }
    }
}
