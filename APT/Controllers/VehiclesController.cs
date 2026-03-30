using APT.Data;
using APT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APT.Controllers
{
    public class VehiclesController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public VehiclesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int buildingId)
        {
            var vehicles = _context.Vehicles
                .Include(v => v.Resident)
                .Include(v => v.Basement)
                .Where(v => v.BuildingId == buildingId)
                .ToList();

            ViewBag.BuildingId = buildingId;

            return View(vehicles);
        }

        public IActionResult Register(int buildingId)
        {
            ViewBag.BuildingId = buildingId;

            ViewBag.Basements = _context.Basements
                .Where(b => b.BuildingId == buildingId)
                .ToList();

            return View();
        }

        [HttpPost]
        public IActionResult Register(Vehicle model)
        {
            var basement = _context.Basements
                .FirstOrDefault(b => b.Id == model.BasementId);

            if (basement == null)
                return NotFound();

            int currentMotorbikes = _context.Vehicles
                .Where(v => v.BasementId == model.BasementId && v.VehicleType == "motorbike")
                .Count();

            int currentCars = _context.Vehicles
                .Where(v => v.BasementId == model.BasementId && v.VehicleType == "car")
                .Count();

            if (model.VehicleType == "motorbike")
            {
                if (currentMotorbikes >= basement.MaxMotorbikes)
                {
                    ModelState.AddModelError("", "Motorbike parking is full");
                    return View(model);
                }
            }

            if (model.VehicleType == "car")
            {
                if (currentCars >= basement.MaxCars)
                {
                    ModelState.AddModelError("", "Car parking is full");
                    return View(model);
                }
            }

            model.CreatedAt = DateTime.Now;

            _context.Vehicles.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Index",
                new { buildingId = model.BuildingId });
        }

        public IActionResult Delete(int id)
        {
            var vehicle = _context.Vehicles.Find(id);

            if (vehicle != null)
            {
                int buildingId = vehicle.BuildingId;

                _context.Vehicles.Remove(vehicle);
                _context.SaveChanges();

                return RedirectToAction("Index",
                    new { buildingId });
            }

            return RedirectToAction("Index");
        }
    }
}