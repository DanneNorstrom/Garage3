namespace Garage3.Models
{
    public class ParkingSpot
    {
        public int Id { get; set; }

        public Vehicle? Vehicle { get; set; } = null!; // Reference navigation to dependent

        public string UserId { get; set; } = string.Empty;

        public DateTime ParkingTime { get; set; } = default!;

    }
}
 