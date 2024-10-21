using System.ComponentModel.DataAnnotations;
using BitirmeProjesi.Models;

namespace BitirmeProjesi.Data {

    public class CarCreateData
    {
        [Key]public int ModelId { get; set; }
        public int BrandId { get; set; }
        public string? ModelName { get; set; }
        public int CategoryId { get; set; }  
        public int Year { get; set; }
        public int Millage { get; set; }
        public int HorsePower { get; set; }
        public int MaxTorque { get; set; }
        public string? Color { get; set; }
        public TransmissionType transmissionType { get; set; }
        public string? Image {get; set;}
        public string? Description {get; set;}
        public int Price { get; set; }
        public DateTime AdvertTime {get; set;}
        public string? Title {get; set;}
    }
}