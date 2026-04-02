using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index(int? building_id)
        {
            // Nếu URL là 0 hoặc null, tự động lấy ID tòa nhà đầu tiên (thường là 1)
            if (building_id == null || building_id == 0)
            {
                var firstBuilding = _context.Buildings.OrderBy(b => b.Id).FirstOrDefault();
                if (firstBuilding != null)
                {
                    return RedirectToAction("Index", new { building_id = firstBuilding.Id });
                }
                return RedirectToAction("Index", "Dashboard");
            }

            // Lấy danh sách từ bảng services
            var services = _context.Services
                .Where(s => s.building_id == building_id)
                .OrderByDescending(s => s.Id)
                .ToList();

            ViewBag.Building = _context.Buildings.Find(building_id);
            ViewBag.BuildingId = building_id;

            return View(services);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Service model)
        {
            ModelState.Remove("Building");

            // Ép kiểu dữ liệu nếu binding lỗi
            if (model.building_id == 0) int.TryParse(Request.Form["building_id"], out int bId);

            if (ModelState.IsValid)
            {
                _context.Services.Add(model);
                _context.SaveChanges();
                TempData["msg_flash"] = "Thêm dịch vụ thành công!";
            }
            return RedirectToAction("Index", new { building_id = model.building_id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Service model)
        {
            ModelState.Remove("Building");
            var db = _context.Services.Find(model.Id);
            if (db != null)
            {
                db.ServiceName = model.ServiceName;
                db.Description = model.Description;
                db.Price = model.Price;
                db.Unit = model.Unit;
                _context.SaveChanges();
                TempData["msg_flash"] = "Cập nhật thành công!";
            }
            return RedirectToAction("Index", new { building_id = model.building_id });
        }
    }
}