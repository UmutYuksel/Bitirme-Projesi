using System.ComponentModel.DataAnnotations;
using BitirmeProjesi.Models;

namespace BitirmeProjesi.Data
{

    public class BrandData
    {
        [Key]
        public int BrandId { get; set; }
        public string? BrandName { get; set; }
        public int CategoryId { get; set; } 
        public Category? Category { get; set; } 

    }
}