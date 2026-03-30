using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using APT.Data;
using APT.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace APT.Controllers
{
    public class ServicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==================================
        // TRANG QUẢN LÝ DỊCH VỤ
        // GET: /services?building_id=1
        // ==================================
        public IActionResult Index(int? building_id)
        {
            var role = HttpContext.Session.GetString("role");
            var userId = HttpContext.Session.GetInt32("user_id");

            // Manager chưa chọn building → auto lấy building đầu tiên được phân công
            if (!building_id.HasValue && role == "manager")
            {
                var assigned = _context.BuildingManagers
                    .Where(bm => bm.ManagerId == userId)
                    .Include(bm => bm.Building)
                    .Select(bm => bm.Building)
                    .ToList();
                if (assigned.Any())
                {
                    return RedirectToAction("Index",
                        new { building_id = assigned.First().Id });
                }

                TempData["msg_flash"] = "Bạn chưa được phân công quản lý tòa nhà nào.";
                return RedirectToAction("Index", "Dashboard");
            }

            if (!building_id.HasValue)
            {
                TempData["msg_flash"] = "Lỗi: Không xác định được tòa nhà!";
                return RedirectToAction("Index", "Apartments");
            }

            var services = _context.Services
                .Where(s => s.BuildingId == building_id)
                .OrderByDescending(s => s.Id)
                .ToList();

            var building = _context.Buildings
                .FirstOrDefault(b => b.Id == building_id);

            ViewBag.Building = building;

            // Chọn view theo role
            if (role == "manager")
                return View("Index_Manager", services);

            return View("Index", services);
        }

        // ==========================
        // THÊM DỊCH VỤ
        // POST: /services/add
        // ==========================
        [HttpPost]
        public IActionResult Add(Service model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index",
                    new { building_id = model.BuildingId });

            _context.Services.Add(model);
            _context.SaveChanges();

            TempData["msg_flash"] = "Thêm dịch vụ thành công!";
            return RedirectToAction("Index",
                new { building_id = model.BuildingId });
        }

        // ==========================
        // CẬP NHẬT DỊCH VỤ
        // POST: /services/update
        // ==========================
        [HttpPost]
        public IActionResult Update(Service model)
        {
            var service = _context.Services.Find(model.Id);
            if (service == null)
            {
                TempData["msg_flash"] = "Không tìm thấy dịch vụ!";
                return RedirectToAction("Index");
            }

            service.Name = model.Name;
            service.Description = model.Description;
            service.Price = model.Price;
            service.Unit = model.Unit;

            _context.SaveChanges();

            TempData["msg_flash"] = "Cập nhật thành công!";
            return RedirectToAction("Index",
                new { building_id = service.BuildingId });
        }

        // ==========================
        // XÓA DỊCH VỤ
        // POST: /services/delete/5
        // ==========================
        [HttpPost]
        public IActionResult Delete(int id, int return_building)
        {
            var service = _context.Services.Find(id);
            if (service != null)
            {
                _context.Services.Remove(service);
                _context.SaveChanges();
                TempData["msg_flash"] = "Đã xóa dịch vụ.";
            }
            else
            {
                TempData["msg_flash"] = "Lỗi khi xóa.";
            }

            return RedirectToAction("Index",
                new { building_id = return_building });
        }
    }
}
