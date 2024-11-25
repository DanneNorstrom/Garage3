using System.ComponentModel.DataAnnotations;

namespace Garage3.Models
{
    public class VehicleType
    {
        public int Id { get; set; }

        [Display(Name = "Vehicle Type")]
        public string Name { get; set; } = string.Empty;
    }
}
