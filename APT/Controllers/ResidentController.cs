using APT.Data;
using APT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace APT.Controllers
{
    public class ResidentsController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public ResidentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =======================================================
        // 1. TRANG CHỦ DÀNH CHO CƯ DÂN (RESIDENT DASHBOARD)
        // =======================================================
        public IActionResult Index()
        {
            int? user_id = HttpContext.Session.GetInt32("user_id");
            if (user_id == null) return RedirectToAction("Login", "Users");

            var resident = _context.Residents
                .Include(r => r.Building)
                .Include(r => r.Apartments)
                .FirstOrDefault(r => r.user_id == user_id);

            if (resident == null) return NotFound("Không tìm thấy thông tin cư dân.");

            var apartment = resident.Apartments?.FirstOrDefault();

            if (apartment != null)
            {
                ViewBag.LatestBill = _context.Bills
                    .Where(b => b.RoomId == apartment.Id && b.Status == false)
                    .OrderByDescending(b => b.CreatedAt)
                    .FirstOrDefault();

                ViewBag.Services = _context.Services
                    .Where(s => s.building_id == resident.building_id)
                    .ToList();

                ViewBag.Apartment = apartment;
            }

            return View("Resident", resident);
        }

        // =======================================================
        // 2. QUẢN LÝ DANH SÁCH CƯ DÂN (DÀNH CHO MANAGER)
        // =======================================================
        public IActionResult ListByBuilding(int building_id)
        {
            var residents = _context.Residents
                .Where(r => r.building_id == building_id)
                .Include(r => r.Apartments)
                .ToList();

            ViewBag.building_id = building_id;
            return View(residents);
        }

        public IActionResult AddToApartment(int apartmentId)
        {
            var apartment = _context.Apartments
                .Include(a => a.Building)
                .FirstOrDefault(a => a.Id == apartmentId);

            if (apartment == null) return NotFound();

            var freeResidents = _context.Residents
                .Where(r => !_context.Apartments.Any(a => a.ResidentId == r.Id))
                .ToList();

            ViewBag.Apartment = apartment;
            ViewBag.FreeResidents = freeResidents;
            ViewBag.building_id = apartment.building_id;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ProcessAddToApartment(Resident model, int apartmentId)
        {
            // Bỏ qua validate các object liên quan để tránh ModelState.IsValid = false
            ModelState.Remove("Building");
            ModelState.Remove("User");
            ModelState.Remove("Apartments");

            if (ModelState.IsValid)
            {
                // Lấy thông tin căn hộ để lấy building_id chính xác
                var apartment = _context.Apartments.Find(apartmentId);
                if (apartment == null) return NotFound();

                // FIX LỖI FOREIGN KEY: Gán building_id từ căn hộ cho cư dân mới
                model.building_id = apartment.building_id;

                _context.Residents.Add(model);
                _context.SaveChanges(); // Lưu Resident trước để lấy ID

                // Gán cư dân vào phòng
                apartment.ResidentId = model.Id;
                apartment.Status = "Occupied";
                _context.SaveChanges();

                TempData["msg_flash"] = "Đã thêm chủ hộ và bàn giao phòng thành công!";
                return RedirectToAction("Index", "Apartments", new { building_id = apartment.building_id });
            }

            // Nếu lỗi, phải nạp lại dữ liệu cho View
            ViewBag.Apartment = _context.Apartments.Include(a => a.Building).FirstOrDefault(a => a.Id == apartmentId);
            return View("AddToApartment", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AssignExistingResident(int residentId, int apartmentId)
        {
            var apartment = _context.Apartments.Find(apartmentId);
            var resident = _context.Residents.Find(residentId);

            if (apartment != null && resident != null)
            {
                // Cập nhật cư dân vào phòng
                apartment.ResidentId = residentId;
                apartment.Status = "Occupied";

                // Đồng bộ building_id của cư dân với tòa nhà của phòng (nếu cần)
                resident.building_id = apartment.building_id;

                _context.SaveChanges();
                TempData["msg_flash"] = "Đã gán cư dân vào phòng thành công!";

                return RedirectToAction("Index", "Apartments", new { building_id = apartment.building_id });
            }
            return RedirectToAction("Dashboard", "Admin");
        }

        // =======================================================
        // 3. CẬP NHẬT & XÓA
        // =======================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Resident model)
        {
            var resident = _context.Residents.Find(model.Id);
            if (resident != null)
            {
                resident.FullName = model.FullName;
                resident.phone = model.phone;
                resident.id_card = model.id_card;
                resident.email = model.email;
                resident.dob = model.dob;
                resident.gender = model.gender;

                _context.SaveChanges();
                TempData["msg_flash"] = "Cập nhật thông tin thành công!";
                return RedirectToAction("ListByBuilding", new { building_id = resident.building_id });
            }
            return RedirectToAction("Dashboard", "Admin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var resident = _context.Residents.Find(id);
            if (resident != null)
            {
                int? bId = resident.building_id;

                // Giải phóng các phòng đang thuê
                var rooms = _context.Apartments.Where(a => a.ResidentId == id).ToList();
                foreach (var r in rooms)
                {
                    r.ResidentId = null;
                    r.Status = "Available";
                }

                _context.Residents.Remove(resident);
                _context.SaveChanges();
                return RedirectToAction("ListByBuilding", new { building_id = bId });
            }
            return RedirectToAction("Dashboard", "Admin");
        }

        [HttpPost]
        public IActionResult RegisterServices(List<int> serviceIds)
        {
            int? user_id = HttpContext.Session.GetInt32("user_id");
            var resident = _context.Residents.Include(r => r.Apartments).FirstOrDefault(r => r.user_id == user_id);
            var apartmentId = resident?.Apartments?.FirstOrDefault()?.Id;

            if (apartmentId != null && serviceIds != null)
            {
                foreach (var sId in serviceIds)
                {
                    _context.ApartmentServices.Add(new ApartmentService
                    {
                        ApartmentId = apartmentId.Value,
                        ServiceId = sId,
                        RegistrationDate = DateTime.Now
                    });
                }
                _context.SaveChanges();
                TempData["msg_flash"] = "Đã đăng ký dịch vụ thành công!";
            }

            return RedirectToAction("Index");
        }
    }
}