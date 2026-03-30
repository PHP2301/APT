using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using APT.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ApartmentMVC.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

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

        // ================= ADMIN =================
        private IActionResult AdminDashboard()
        {
            var data = new
            {
                total_buildings = _context.Buildings.Count(),
                total_rooms = _context.Apartments.Count(),
                total_users = _context.Users.Count(),
                chartData = _context.Apartments
                    .GroupBy(a => a.Building_id)
                    .Select(g => new
                    {
                        buildingId = g.Key,
                        total = g.Count()
                    })
                    .ToList()
            };

            ViewBag.Data = data;
            return View("Admin");
        }

        // ================= MANAGER =================
        private IActionResult ManagerDashboard()
        {
            int userId = HttpContext.Session.GetInt32("user_id") ?? 0;

            var user = _context.Users.Find(userId);

            var buildings = _context.BuildingManagers
                .Where(bm => bm.ManagerId == userId)
                .Include(bm => bm.Building)
                .Select(bm => bm.Building)
                .ToList();

            ViewBag.User = user;
            ViewBag.Buildings = buildings;

            return View("Manager");
        }


        // ================= RESIDENT =================
        private IActionResult ResidentDashboard()
        {
            int userId = HttpContext.Session.GetInt32("user_id") ?? 0;

            var resident = _context.Residents
                .Include(r => r.Building)
                .FirstOrDefault(r => r.UserId == userId);

            var bills = _context.Bills
                .Where(b => b.Apartment.ResidentId == resident.Id)
                .ToList();

            ViewBag.Bills = bills;
            ViewBag.Services = _context.Services
                .Where(s => s.BuildingId == resident.BuildingId)
                .ToList();

            return View("Resident");
        }
    }
}
