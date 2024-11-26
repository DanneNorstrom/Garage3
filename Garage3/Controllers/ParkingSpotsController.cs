using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Garage3.Models;
using Garage3.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace dbtest3.Controllers
{
    public class ParkingSpotsController : Controller
    {
        private readonly Garage3Context _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ParkingSpotsController(Garage3Context context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;


            /*if(!_context.VehicleTypes.Any(e => e.Id == 1))
            {
                VehicleType vt1 = new VehicleType(); vt1.Id = 1; vt1.Name = "Car";
                _context.VehicleTypes.Add(vt1);

                VehicleType vt2 = new VehicleType(); vt2.Id = 2; vt2.Name = "Motorcycle";
                _context.VehicleTypes.Add(vt2);

                VehicleType vt3 = new VehicleType(); vt3.Id = 3; vt3.Name = "Truck";
                _context.VehicleTypes.Add(vt3);

                VehicleType vt4 = new VehicleType(); vt4.Id = 4; vt4.Name = "Plane";
                _context.VehicleTypes.Add(vt4);

                VehicleType vt5 = new VehicleType(); vt5.Id = 5; vt5.Name = "Boat";
                _context.VehicleTypes.Add(vt5);

                _context.SaveChanges();
            }*/
        }

        // GET: ParkingSpots
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var parkingSpots = await _context.ParkingSpots.
                Include(ps => ps.Vehicle).
                ThenInclude(ps => ps.VehicleType).
                OrderBy(ps => ps.Id).
                ToListAsync();

            var psDetailsVm =  new List<PsDetailsViewModel>();


            foreach (var p in parkingSpots)
            { 
                var psd = new PsDetailsViewModel();
                psd.Id = p.Id;

                if (p.Vehicle != null)
                {
                    psd.isEmpty = false;      
                    psd.RegNr = p.Vehicle.RegNr;
                    psd.VehicleTypeName = p.Vehicle.VehicleType.Name;

                    if (User.IsInRole("Admin") || p.UserId == _userManager.GetUserId(this.User))
                    {
                        psd.isCheckoutable = true;

                        var ts = DateTime.Now.Subtract(p.ParkingTime);
                        psd.ParkingTime = "Hours: " + (ts.Days * 24 + ts.Hours) + " " + "Minutes: " + ts.Minutes;

                        var user = await _userManager.FindByIdAsync(p.UserId);
                        psd.UserFullName = user.FirstName + " " + user.LastName;
                    }

                    else
                    {
                        psd.isCheckoutable = false;
                    }
                }

                else
                {
                    psd.isEmpty = true;
                }

                psDetailsVm.Add(psd);
            }

            return View(psDetailsVm);
        }

        // GET: ParkingSpots/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ParkingSpots/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id")] ParkingSpot parkingSpot)
        {
            if (ModelState.IsValid)
            {
                _context.ParkingSpots.Add(parkingSpot);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(parkingSpot);
        }

        // GET: ParkingSpots/Park/5
        public async Task<IActionResult> Park(int? id)
        {
            var pvm = new ParkViewModel();
            pvm.Id = (int)id;

            pvm.VehicleTypesNames = await _context.VehicleTypes.
                Select(vt => vt.Name).ToListAsync();

            if (pvm.VehicleTypesNames == null)
            {
                return NotFound();
            }

            return View(pvm);
        }

        // POST: ParkingSpots/Park/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Park([Bind("Id,VehicleTypeName,RegNr")] ParkViewModel pvm)
        {
            if (ModelState.IsValid)
            {
                var ps = new ParkingSpot()
                {   
                    Id = pvm.Id,
                    ParkingTime = DateTime.Now
                };

                try
                {
                    ps.Vehicle = new Vehicle
                    {
                        RegNr = pvm.RegNr,
                        VehicleType = await _context.VehicleTypes.
                        FirstOrDefaultAsync(t => t.Name == pvm.VehicleTypeName)
                    };

                    if (ps.Vehicle.VehicleType == null)
                    {
                        return NotFound();
                    }

                    var userId = _userManager.GetUserId(this.User);
                    ps.UserId = userId;

                    _context.ParkingSpots.Update(ps);
                    _context.Vehicles.Add(ps.Vehicle);
                    await _context.SaveChangesAsync();
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!ParkingSpotExists(ps.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        // GET: ParkingSpots/Checkout/5
        public async Task<IActionResult> Checkout(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ps = await _context.ParkingSpots
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ps == null)
            {
                return NotFound();
            }

            return View(ps);
        }

        // POST: ParkingSpots/Checkout/5
        [HttpPost, ActionName("Checkout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckoutConfirmed(int id)
        {
            var parkingSpot = await _context.ParkingSpots.
                Include(ps => ps.Vehicle).
                FirstOrDefaultAsync(ps => ps.Id == id);

            if (parkingSpot != null)
            {
                _context.Vehicles.Remove(parkingSpot.Vehicle);
                parkingSpot.Vehicle = null;
                _context.ParkingSpots.Update(parkingSpot);
                parkingSpot.UserId = string.Empty;
                parkingSpot.ParkingTime = default;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ParkingSpotExists(int id)
        {
            return _context.ParkingSpots.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Search(string vehicleType)
        { 
            var psMatches = await _context.ParkingSpots.
                               
            OrderBy(ps => ps.Id).
            Select(ps => new SearchViewModel
            {
                Id = ps.Id,
                RegNr = ps.Vehicle.RegNr,
                VehicleTypeName = ps.Vehicle.VehicleType.Name
            }).
            Where(ps => ps.VehicleTypeName == vehicleType).
            ToListAsync();
   
            return View(psMatches);
        }
    }
}
