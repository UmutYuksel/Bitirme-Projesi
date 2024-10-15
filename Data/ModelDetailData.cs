using System.ComponentModel.DataAnnotations;

namespace BitirmeProjesi.Data
{

    public class ModelDetailsData
    {
        [Key]
        public int CarId { get; set; }
        public int Year { get; set; }
        public int Millage { get; set; }
        public int HorsePower { get; set; }
        public int MaxTorque { get; set; }
        public required string Color { get; set; }
        public TransmissionType TransmissionType { get; set; }
    }

}