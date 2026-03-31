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
                total = _context.Apartments.Count(a => a.BuildingId == b.Id)
            }).ToList();

            // Cập nhật ViewBag.Data với đầy đủ các thuộc tính
            ViewBag.Data = new
            {
                total_buildings = totalBuildings,
                total_rooms = totalRooms,
                total_users = totalUsers, // 🔥 Phải có dòng này thì View mới gọi được
                chartData = chartData
            };

            return View();
        }
    }
}