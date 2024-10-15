using System.ComponentModel.DataAnnotations;


namespace BitirmeProjesi.Models
{

    public class Category
    {
        [Key]
        [Display(Name = "Kategori ID")]
        public int CategoryId { get; set; }
        
        [Display(Name = "Kategori Adı")]
        [Required(ErrorMessage = "Kategori Adı Eksik")]
        [StringLength(100)]
        public string? CategoryName { get; set; }

        public ICollection<Brand>? Brands { get; set; } // Kategoriye bağlı markalar
    }
}