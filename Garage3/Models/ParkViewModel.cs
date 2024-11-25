namespace Garage3.Models
{
    public class ParkViewModel
    {
        public int Id { get; set; }
        public string RegNr { get; set; }

        public List<string> VehicleTypesNames = null!;         
        public string VehicleTypeName { get; set; }
    }
}
