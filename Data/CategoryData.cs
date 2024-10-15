using System.ComponentModel.DataAnnotations;

namespace BitirmeProjesi.Data
{

    public class CategoryData
    {
        [Key]
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
    }
}