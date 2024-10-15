using System.ComponentModel.DataAnnotations;

namespace BitirmeProjesi.Models
{

    public class ModelDetails
    {
        [Key]
        [Display(Name = "Araç Idsi")]
        public int CarId { get; set; }

        [Required]
        [Display(Name = "Araç Yılı")]
        public int Year { get; set; }

        [Required]
        [Display(Name = "Kilometre")]
        public int Millage { get; set; }

        [Required]
        [Display(Name = "Beygir Gücü")]
        public int HorsePower { get; set; }

        [Required]
        [Display(Name = "Maksimum Tork")]
        public int MaxTorque { get; set; }

        [Required]
        [Display(Name = "Araç Rengi")]
        public string? Color { get; set; }

        [Required]
        [Display(Name = "Şanzıman Tipi")]
        public TransmissionType transmissionType { get; set; }
    }
}

public enum TransmissionType
{
    Manuel,
    Otomatik,
    Yarı_Otomatik,
    CVT
}
