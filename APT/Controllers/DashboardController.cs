using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using APT.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using APT.Models;

namespace ApartmentMVC.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ĐIỀU HƯỚNG TRANG CHỦ THEO ROLE
        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("user_id") == null)
                return RedirectToAction("Login", "Users");

            var role = (HttpContext.Session.GetString("role") ?? "").ToLower();

            return role switch
            {
                "super_admin" or "admin" => AdminDashboard(),
                "manager" => ManagerDashboard(),
                "resident" => ResidentDashboard(),
                _ => Logout()
            };
        }

        private IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Users");
        }

        // ================= 1. ADMIN DASHBOARD =================
        private IActionResult AdminDashboard()
        {
            var data = new
            {
                total_buildings = _context.Buildings.Count(),
                total_rooms = _context.Apartments.Count(),
                total_users = _context.Users.Count(),
                chartData = _context.Apartments
                    .GroupBy(a => a.building_id)
                    .Select(g => new { building_id = g.Key, total = g.Count() })
                    .ToList()
            };

            ViewBag.Data = data;
            return View("Admin");
        }

        // ================= 2. QUẢN LÝ NHÂN SỰ (Dành cho Admin) =================
        // Đổi tên từ Manager() thành Personnel() để tránh xung đột
        public IActionResult ManagerList()
        {
            if (HttpContext.Session.GetInt32("user_id") == null)
                return RedirectToAction("Login", "Users");

            var users = _context.Users.Where(u => u.Role == "manager").ToList();

            // Gọi đúng tên file ManagerList
            return View("ManagerList", users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddManager(User model)
        {
            model.Password = BCrypt.Net.BCrypt.HashPassword("Quanly@123"); // Băm mật khẩu cho bảo mật
            model.Role = "manager";
            model.CreatedAt = DateTime.Now;

            ModelState.Remove("Password");
            ModelState.Remove("Role");
            ModelState.Remove("CreatedAt");

            if (ModelState.IsValid)
            {
                _context.Users.Add(model);
                _context.SaveChanges();
                return RedirectToAction("ManagerList"); // Quay lại đúng trang danh sách
            }
            return View("ManagerList", _context.Users.Where(u => u.Role == "manager").ToList());
        }

        // ================= 3. MANAGER DASHBOARD (Trang chủ của Manager) =================
        private IActionResult ManagerDashboard()
        {
            int user_id = HttpContext.Session.GetInt32("user_id") ?? 0;
            var user = _context.Users.Find(user_id);

            // Truy vấn thông qua bảng trung gian building_managers
            var buildings = _context.Building_Managers // Nhớ map Table("building_managers") trong Model như em chỉ nhé
                .Where(bm => bm.ManagerId == user_id)
                .Include(bm => bm.Building)
                .Select(bm => bm.Building)
                .ToList();

            ViewBag.User = user;

            // Nếu buildings rỗng, trang web sẽ hiện cái khung trắng như trong ảnh của Đại Ca
            return View("Manager", buildings);
        }

        // ================= 4. RESIDENT DASHBOARD =================
        private IActionResult ResidentDashboard()
        {
            int user_id = HttpContext.Session.GetInt32("user_id") ?? 0;
            var resident = _context.Residents
                .Include(r => r.Building)
                .FirstOrDefault(r => r.user_id == user_id);

            if (resident != null)
            {
                ViewBag.Bills = _context.Bills.Where(b => b.Apartment.ResidentId == resident.Id).ToList();
                ViewBag.Services = _context.Services.Where(s => s.building_id == resident.building_id).ToList();
            }

            return View("Resident");
        }
    }
}