//using APT.Data;
//using APT.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Linq;

//namespace APT.Controllers
//{
//    public class VehiclesController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public VehiclesController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        // 1. TRANG DANH SÁCH XE CỦA TÒA NHÀ
//        public IActionResult Index(int buildingId)
//        {
//            var building = _context.Buildings.Find(buildingId);
//            ViewBag.BuildingName = building?.Name ?? "Tòa nhà";
//            ViewBag.BuildingId = buildingId;

//            // CHỐT HẠ: Chỉ Include Apartment, tuyệt đối KHÔNG Include Basement hay Resident ở đây
//            var vehicles = _context.Vehicles
//                .Include(v => v.Apartment)
//                .Where(v => v.building_id == buildingId) // Dùng trực tiếp building_id trong bảng Vehicles cho lẹ
//                .ToList();

//            return View(vehicles);
//        }

//        // 2. TRANG ĐĂNG KÝ XE MỚI
//        public IActionResult Register(int buildingId)
//        {
//            ViewBag.BuildingId = buildingId;

//            // Lấy danh sách căn hộ để chọn khi đăng ký xe
//            ViewBag.Apartments = _context.Apartments
//                .Where(a => a.building_id == buildingId)
//                .ToList();

//            return View();
//        }

//        // 3. XỬ LÝ LƯU ĐĂNG KÝ XE
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public IActionResult Register(Vehicle model)
//        {
//            // Xóa bỏ kiểm tra các quan hệ phức tạp có thể gây lỗi ModelState
//            ModelState.Remove("Apartment");
//            ModelState.Remove("Building");

//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    // Đảm bảo gán đúng BuildingId từ form hoặc từ Apartment
//                    var apartment = _context.Apartments.Find(model.ApartmentId);
//                    if (apartment != null)
//                    {
//                        model.building_id = apartment.building_id;
//                    }

//                    _context.Vehicles.Add(model);
//                    _context.SaveChanges();

//                    TempData["msg_flash"] = "Đăng ký xe thành công!";
//                    return RedirectToAction("Index", new { buildingId = model.building_id });
//                }
//                catch (Exception ex)
//                {
//                    ModelState.AddModelError("", "Lỗi hệ thống: " + ex.Message);
//                }
//            }

//            ViewBag.BuildingId = model.building_id;
//            ViewBag.Apartments = _context.Apartments.Where(a => a.building_id == model.building_id).ToList();
//            return View(model);
//        }

//        // 4. XÓA XE
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public IActionResult Delete(int id)
//        {
//            var vehicle = _context.Vehicles.Find(id);

//            if (vehicle != null)
//            {
//                int bId = vehicle.building_id;
//                _context.Vehicles.Remove(vehicle);
//                _context.SaveChanges();
//                TempData["msg_flash"] = "Đã xóa xe thành công.";
//                return RedirectToAction("Index", new { buildingId = bId });
//            }

//            return RedirectToAction("Index", "Dashboard");
//        }
//    }
//}