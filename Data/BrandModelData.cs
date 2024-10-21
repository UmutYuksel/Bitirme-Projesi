using System.ComponentModel.DataAnnotations;
using BitirmeProjesi.Models;

namespace BitirmeProjesi.Data
{

    public class BrandModelData
    {

        [Key]
        public int ModelId { get; set; }
        public int BrandId { get; set; }
        public string? ModelName { get; set; }
        public int HorsePower { get; set; }
        public int MaxTorque { get; set; }
        public int CategoryId { get; set; }
    }
}