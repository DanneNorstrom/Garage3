using Garage3.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Garage3.Data
{
    public class Garage3Context : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public Garage3Context (DbContextOptions<Garage3Context> options)
            : base(options)
        {
        }

        public DbSet<ParkingSpot> ParkingSpots { get; set; } = default!;
        public DbSet<Vehicle> Vehicles { get; set; } = default!;
        public DbSet<VehicleType> VehicleTypes { get; set; } = default!;
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Garage3.Models.PsOverviewModel> PsOverviewModel { get; set; } = default!;

    }
}
