namespace Garage3.Models
{
    public class ParkViewModel
    {
        public string RegNr { get; set; }

        public int Id { get; set; }
        
        public List<string> VehicleTypesNames = null!;         
        public string VehicleTypeName { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;
    }
}
