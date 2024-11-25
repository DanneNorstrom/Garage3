namespace Garage3.Models
{
    public class PsDetailsViewModel
    {
        public int Id { get; set; }

        public bool isEmpty = true;

        public bool isCheckoutable = false;
        public string RegNr { get; set; } = string.Empty;
        public string VehicleTypeName { get; set; } = string.Empty;
        public string ParkingTime { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;
    }
}
