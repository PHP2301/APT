using APT.Data;
using APT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; // Thêm để dùng async/await

namespace APT.Controllers
{
    public class ResidentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ResidentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. DANH SÁCH CƯ DÂN (DÀNH CHO ADMIN) - ĐÃ SỬA ĐỂ HIỆN DỊCH VỤ
        // ==========================================
        public IActionResult Index(int building_id)
        {
            if (building_id == 0)
            {
                building_id = HttpContext.Session.GetInt32("current_building_id") ?? 1;
            }

            // --- SỬA TẠI ĐÂY ---
            // Thêm Include để lấy chuỗi: User -> Apartments -> ApartmentServices -> Service
            var residents = _context.Users
                .Include(u => u.Apartments)
                    .ThenInclude(a => a.ApartmentServices) // Lấy bảng trung gian đăng ký dịch vụ
                    .ThenInclude(aps => aps.Service)       // Lấy thông tin chi tiết dịch vụ (Tên, Giá)
                .Where(u => u.Role == "resident")
                .ToList();

            // Lấy danh sách phòng trống để hiển thị trong Modal "Vào phòng"
            ViewBag.EmptyRooms = _context.Apartments
                .Where(a => a.building_id == building_id && (a.ResidentId == null || a.Status == "Available"))
                .ToList();

            ViewBag.building_id = building_id;
            return View(residents);
        }

        // ==========================================
        // 2. MỞ TRANG THÊM/SỬA CƯ DÂN (GET)
        // ==========================================
        [HttpGet]
        public IActionResult Edit(int? id, int building_id)
        {
            ViewBag.building_id = building_id;

            if (id == null || id == 0)
            {
                return View(new User());
            }

            var user = _context.Users.Find(id);
            if (user == null) return NotFound();

            return View(user);
        }

        // ==========================================
        // 3. XỬ LÝ LƯU DỮ LIỆU CƯ DÂN (POST)
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(User model, int? building_id, int? apartment_id)
        {
            ModelState.Clear();
            int finalBId = building_id ?? 1;

            try
            {
                if (model.Id == 0)
                {
                    model.Role = "resident";
                    model.Password = "123456";
                    model.CreatedAt = DateTime.Now;

                    _context.Users.Add(model);
                    _context.SaveChanges();

                    if (apartment_id.HasValue && apartment_id > 0)
                    {
                        var apt = _context.Apartments.Find(apartment_id.Value);
                        if (apt != null)
                        {
                            apt.ResidentId = model.Id;
                            apt.Status = "Occupied";
                            _context.SaveChanges();
                            TempData["msg_flash"] = $"Đã thêm {model.FullName} và bàn giao phòng {apt.FullRoomName}!";
                        }
                    }
                    else
                    {
                        TempData["msg_flash"] = "Thêm cư dân mới thành công!";
                    }
                }
                else
                {
                    var userInDb = _context.Users.Find(model.Id);
                    if (userInDb == null) return NotFound();

                    userInDb.FullName = model.FullName;
                    userInDb.Phone = model.Phone;
                    userInDb.Email = model.Email;
                    userInDb.IdCard = model.IdCard;
                    userInDb.Gender = model.Gender;
                    userInDb.Dob = model.Dob;

                    _context.SaveChanges();
                    TempData["msg_flash"] = "Cập nhật hồ sơ thành công!";
                }

                return RedirectToAction("Index", "Apartments", new { building_id = finalBId });
            }
            catch (Exception ex)
            {
                TempData["msg_flash"] = "Lỗi Database: " + ex.Message;
                ViewBag.building_id = finalBId;
                return View(model);
            }
        }

        // ==========================================
        // 4. TRANG CÁ NHÂN CỦA CƯ DÂN (DASHBOARD)
        // ==========================================
        public IActionResult User()
        {
            int? userId = HttpContext.Session.GetInt32("user_id");
            if (userId == null) return RedirectToAction("Login", "Users");

            var resident = _context.Users
                .Include(u => u.Apartments)
                    .ThenInclude(a => a.ApartmentServices)
                    .ThenInclude(aps => aps.Service)
                .FirstOrDefault(u => u.Id == userId);

            if (resident == null) return NotFound();

            var apartment = resident.Apartments?.FirstOrDefault();

            if (apartment != null)
            {
                var bill = _context.Bills
                    .Where(b => b.RoomId == apartment.Id && b.Status == false)
                    .OrderByDescending(b => b.CreatedAt)
                    .FirstOrDefault();

                if (bill == null)
                {
                    bill = _context.Bills
                        .Where(b => b.RoomId == apartment.Id)
                        .OrderByDescending(b => b.CreatedAt)
                        .FirstOrDefault();
                }

                ViewBag.LatestBill = bill;
                ViewBag.Apartment = apartment;
            }

            ViewBag.Services = _context.Services.ToList();
            return View(resident);
        }

        // ==========================================
        // 5. XÓA CƯ DÂN (GIẢI PHÓNG PHÒNG)
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var user = _context.Users.Include(u => u.Apartments).FirstOrDefault(u => u.Id == id);

            if (user != null)
            {
                int bId = user.Apartments.FirstOrDefault()?.building_id ?? 1;

                foreach (var apt in user.Apartments)
                {
                    apt.ResidentId = null;
                    apt.Status = "Available";
                }

                _context.Users.Remove(user);
                _context.SaveChanges();

                TempData["msg_flash"] = "Đã xóa cư dân và giải phóng phòng!";
                return RedirectToAction("Index", "Apartments", new { building_id = bId });
            }
            return RedirectToAction("Index", "Dashboard");
        }

        // ==========================================
        // 6. GÁN CƯ DÂN VÀO PHÒNG (DÀNH CHO MODAL)
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AssignRoom(int residentId, int apartmentId, int building_id)
        {
            var apartment = _context.Apartments.Find(apartmentId);
            var resident = _context.Users.Find(residentId);

            if (apartment != null && resident != null)
            {
                apartment.ResidentId = residentId;
                apartment.Status = "Occupied";
                _context.SaveChanges();
                TempData["msg_flash"] = $"Đã gán {resident.FullName} vào phòng {apartment.FullRoomName}!";
            }
            return RedirectToAction("Index", new { building_id = building_id });
        }
    }
}