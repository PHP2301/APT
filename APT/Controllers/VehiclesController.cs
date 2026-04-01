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
            // 1. Lấy thông tin tòa nhà
            var building = _context.Buildings.Find(buildingId);
            ViewBag.BuildingName = building?.Name ?? "IDICO";

            // 2. Lấy danh sách hầm kèm theo danh sách xe trong mỗi hầm để đếm
            var basements = _context.Basements
                .Where(b => b.BuildingId == buildingId)
                .Include(b => b.Vehicles) // Cần nạp danh sách xe để đếm ở View
                .ToList();

            ViewBag.BuildingId = buildingId;
            return View(basements); // Trả về danh sách Basements
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
        [HttpPost]
        public IActionResult UpdateBasement(Basement model)
        {
            var basement = _context.Basements.Find(model.Id);
            if (basement != null)
            {
                basement.MaxMotorbikes = model.MaxMotorbikes;
                basement.MaxCars = model.MaxCars;
                // Nếu DB có cột giá vé thì Đại Ca gán thêm ở đây
                // basement.MotorbikePrice = model.MotorbikePrice;
                // basement.CarPrice = model.CarPrice;

                _context.SaveChanges();
                TempData["msg_flash"] = "Cập nhật cấu hình hầm thành công!";
            }
            return RedirectToAction("Index", new { buildingId = basement?.BuildingId });
        }
    }
}