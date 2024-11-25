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
        }

        // GET: ParkingSpots
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var parkingSpots = await _context.ParkingSpots.
                Include(ps => ps.Vehicle).
                ThenInclude(ps => ps.VehicleType).
                //Include(ps => ps.Member).
                OrderBy(ps => ps.Id).
                ToListAsync();

            foreach(var p in parkingSpots)
            {
                if (p.Vehicle != null)
                {
                    var usr = await _userManager.FindByIdAsync(p.UserId);
                    //p.UserId = usr.Id +" " + usr.FirstName + " " + usr.LastName + " " + p.ParkingTime.ToString() + " " + _userManager.GetUserId(this.User);
                    p.UserId = usr.Id;
                    p.Vehicle.VehicleType.Name += " " + usr.FirstName + " " + usr.LastName + " " + p.ParkingTime.ToString();
                    //p.Vehicle.Owner = usr.PersonNumber + " " + usr.FirstName +" " + usr.LastName;
                }
            }

            ViewData["curruserid"] = _userManager.GetUserId(this.User);

            return View(parkingSpots);
        }

        // GET: ParkingSpots
        [Authorize]
        public async Task<IActionResult> OverView()
        {
            var parkingSpots = await _context.ParkingSpots.
                Include(ps => ps.Vehicle).
                ThenInclude(ps => ps.VehicleType).
                OrderBy(ps => ps.Id).
                ToListAsync();

            List<PsOverviewModel> parkingSpotsOverView = null!;
            parkingSpotsOverView = new List<PsOverviewModel>();


            foreach (var p in parkingSpots)
            {
                var pso = new PsOverviewModel();
                pso.Id = p.Id;

                if (p.Vehicle != null)
                {
                    var usr = await _userManager.FindByIdAsync(p.UserId);

                    pso.RegNr = p.Vehicle.RegNr;
                    pso.VehicleTypeName = p.Vehicle.VehicleType.Name;
                    pso.ParkingTime = p.ParkingTime.ToString();
                    pso.UserId = p.UserId;
                    var user = await _userManager.FindByIdAsync(p.UserId);
                    pso.UserFullName = user.FirstName + " " + user.LastName;
                }

                parkingSpotsOverView.Add(pso);

            }

            ViewData["curruserid"] = _userManager.GetUserId(this.User);

            return View(parkingSpotsOverView);
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

                    /*ps.Member = new Member
                    {
                        Name = pvm.Name
                    };*/

                    //_context.Members.Add(ps.Member);
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
                //return View();
            }
            return View();
        }

        // GET: ParkingSpots/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: ParkingSpots/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var parkingSpot = await _context.ParkingSpots.
                Include(ps => ps.Vehicle).
                //Include(ps => ps.Member).
                FirstOrDefaultAsync(ps => ps.Id == id);

            if (parkingSpot != null)
            {
                _context.Vehicles.Remove(parkingSpot.Vehicle);
                parkingSpot.Vehicle = null;
                _context.ParkingSpots.Update(parkingSpot);
                parkingSpot.UserId = string.Empty;
                parkingSpot.ParkingTime = default;
                //_context.Members.Remove(parkingSpot.Member);
                //parkingSpot.Member = null;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ParkingSpotExists(int id)
        {
            return _context.ParkingSpots.Any(e => e.Id == id);
        }
    }
}
