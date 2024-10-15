using System.ComponentModel.DataAnnotations;
using BitirmeProjesi.Models;

namespace BitirmeProjesi.Data
{

    public class BrandData
    {

        [Key]
        public int BrandId { get; set; }
        public string? BrandName { get; set; }
        // Foreign key
        public int CategoryId { get; set; }

        // Navigation property
        public virtual Category? Category { get; set; }

    }
}