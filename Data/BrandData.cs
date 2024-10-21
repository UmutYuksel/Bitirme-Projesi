using System.ComponentModel.DataAnnotations;
using BitirmeProjesi.Models;

namespace BitirmeProjesi.Data
{

    public class BrandData
    {
        [Key]
        public int BrandId { get; set; }
        public string? BrandName { get; set; }
        public int CategoryId { get; set; } // Hangi kategoriye ait olduğunu belirtiyor
        public Category? Category { get; set; } // İlişki

    }
}