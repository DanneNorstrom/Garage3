namespace Garage3.Models
{
    public class PsOverviewModel
    {
        public int Id { get; set; }
        public string RegNr { get; set; } = string.Empty;
        public string VehicleTypeName { get; set; } = string.Empty;
        public string ParkingTime { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;
    }
}
