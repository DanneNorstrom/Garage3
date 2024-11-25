using System.ComponentModel.DataAnnotations;

namespace Garage3.Models
{
    public class Vehicle
    {
        [Key]
        public string RegNr { get; set; } = string.Empty;
        public VehicleType VehicleType { get; set; } = null!; // Reference navigation to dependent
        public string Owner { get; set; } = string.Empty;

    }
}