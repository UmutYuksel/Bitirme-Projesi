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

        [Display(Name = "İlan Resmi")]
        [Required(ErrorMessage = "İlan Resmi Eksik")]
        public string? Image {get; set;}

        [Display(Name = "İlan Açıklaması")]
        [Required(ErrorMessage = "İlan Açıklaması Eksik")]
        public string? Description {get; set;}

        [Display(Name = "İlan Fiyatı")]
        [Required(ErrorMessage = "İlan Fiyatı Eksik")]
        public int Price { get; set; }

        [Display(Name = "İlan Tarihi")]
        [Required(ErrorMessage = "İlan Tarihi Eksik")]
        public DateTime AdvertTime {get; set;}

        [Display(Name = "İlan Başlığı")]
        [Required(ErrorMessage = "İlan Başlığı Eksik")]
        public string? Title {get; set;}
    }
}

    public enum TransmissionType
    {
        Manuel,
        Otomatik,
        Yarı_Otomatik,
        CVT
    } 
