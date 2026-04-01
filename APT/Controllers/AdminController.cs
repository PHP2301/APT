using APT.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace APT.Controllers
{
    public class AdminController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            // Lấy dữ liệu từ Database
            var totalBuildings = _context.Buildings.Count();
            var totalRooms = _context.Apartments.Count();
            var totalUsers = _context.Users.Count(); // 🔥 Thêm dòng này để đếm nhân sự

            var chartData = _context.Buildings.Select(b => new {
                name = b.Name,
                total = _context.Apartments.Count(a => a.building_id == b.Id) 
            }).ToList();

            ViewBag.Data = new
            {
                total_buildings = totalBuildings,
                total_rooms = totalRooms,
                total_users = totalUsers,
                chartData = chartData
            };

            return View("~/Views/Dashboard/Admin.cshtml");
        }
    }
}