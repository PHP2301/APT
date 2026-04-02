using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using APT.Data;
using APT.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using System;

namespace ApartmentMVC.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// NGÃ BA ĐIỀU HƯỚNG CHÍNH
        /// </summary>
        public IActionResult Index()
        {
            var role = (HttpContext.Session.GetString("role") ?? "").ToLower();

            if (role == "admin" || role == "super_admin")
            {
                return AdminDashboard();
            }
            else if (role == "User")
            {
                return ResidentDashboard();
            }

            return RedirectToAction("Login", "Users");
        }

        // ================= 1. ADMIN DASHBOARD (TRANG THỐNG KÊ) =================
        private IActionResult AdminDashboard()
        {
            var data = new
            {
                total_buildings = _context.Buildings.Count(),
                total_rooms = _context.Apartments.Count(),
                total_users = _context.Users.Count(),
                chartData = _context.Apartments
                    .GroupBy(a => a.building_id)
                    .Select(g => new {
                        name = _context.Buildings.Where(b => b.Id == g.Key).Select(b => b.Name).FirstOrDefault() ?? "N/A",
                        total = g.Count()
                    })
                    .ToList()
            };

            ViewBag.Data = data;
            return View("Admin");
        }

        // ================= 2. RESIDENT DASHBOARD =================
        private IActionResult ResidentDashboard()
        {
            int user_id = HttpContext.Session.GetInt32("user_id") ?? 0;

            // Lấy thông tin User hiện tại kèm theo Apartment của họ
            var userResident = _context.Users
                .Include(u => u.Apartments)
                .FirstOrDefault(u => u.Id == user_id);

            if (userResident != null)
            {
                var apartment = userResident.Apartments.FirstOrDefault();
                if (apartment != null)
                {
                    ViewBag.Bills = _context.Bills.Where(b => b.RoomId == apartment.Id).ToList();
                    ViewBag.Services = _context.Services.Where(s => s.building_id == apartment.building_id).ToList();
                    ViewBag.Apartment = apartment;
                }
            }

            return View("Resident", userResident);
        }

        // ================= 3. CHỌN TÒA NHÀ ĐỂ QUẢN LÝ =================
        public IActionResult ChooseBuilding()
        {
            var role = (HttpContext.Session.GetString("role") ?? "").ToLower();
            if (role != "admin" && role != "super_admin")
            {
                return RedirectToAction("Index");
            }

            var buildings = _context.Buildings.ToList();

            // Trỏ về View Choose mà anh em mình vừa làm
            return View("~/Views/Buildings/Choose.cshtml", buildings);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Users");
        }
    }
}