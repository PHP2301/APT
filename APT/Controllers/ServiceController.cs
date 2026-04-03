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
        public async Task<IActionResult> Add(Service service, int building_id)
        {
            service.building_id = building_id;
            _context.services.Add(service);
            await _context.SaveChangesAsync();
            TempData["msg_flash"] = "Đã thêm dịch vụ thành công!";
            return RedirectToAction("Index", new { building_id });
        }

        [HttpPost]
        public async Task<IActionResult> Update(Service service, int building_id)
        {
            _context.services.Update(service);
            await _context.SaveChangesAsync();
            TempData["msg_flash"] = "Cập nhật thành công!";
            return RedirectToAction("Index", new { building_id });
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id, int building_id)
        {
            var service = await _context.services.FindAsync(id);
            if (service != null)
            {
                _context.services.Remove(service);
                await _context.SaveChangesAsync();
                TempData["msg_flash"] = "Đã xóa dịch vụ.";
            }
            return RedirectToAction("Index", new { building_id });
        }
    }
}