using System.ComponentModel.DataAnnotations;
using BitirmeProjesi.Models;

namespace BitirmeProjesi.Data
{

    public class CategoryData
    {
        [Key]
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public ICollection<Brand>? Brands { get; set; }
    }
}