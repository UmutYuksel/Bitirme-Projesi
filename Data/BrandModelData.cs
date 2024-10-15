using System.ComponentModel.DataAnnotations;

namespace BitirmeProjesi.Data
{

    public class BrandModelData
    {

        [Key]
        public int ModelId { get; set; }
        public int BrandId { get; set; }
        public string? ModelName { get; set; }
        public int CategoryId { get; set; }
    }
}