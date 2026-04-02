using APT.Data;
using APT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace APT.Controllers
{
    public class BuildingsController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public BuildingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. TRANG QUẢN LÝ (Giao diện cái bảng trắng - Dùng khi cần quản lý sâu)
        // URL: /Buildings/Index
        public IActionResult Index()
        {
            return RedirectToAction("Choose");
        }

        // 2. TRANG CHỌN TÒA NHÀ (Giao diện Card tròn đẹp như Đại Ca muốn)
        // URL: /Buildings/Choose
        public IActionResult Choose()
        {
            var buildings = _context.Buildings.ToList();
            return View(buildings); // Sẽ tìm file Views/Buildings/Choose.cshtml
        }

        // 3. XỬ LÝ CHỌN TÒA NHÀ
        public IActionResult ChooseBuilding(int id)
        {
            HttpContext.Session.SetInt32("current_building_id", id);
            return RedirectToAction("Index", "Apartments", new { building_id = id });
        }

        // =======================================================
        // 4. TẠO MỚI TÒA NHÀ
        // =======================================================

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Building model)
        {
            if (ModelState.IsValid)
            {
                _context.Buildings.Add(model);
                _context.SaveChanges();
                TempData["msg_flash"] = "Thêm tòa nhà mới thành công!";

                // SAU KHI TẠO: Nhảy về trang CHOOSE (Card) thay vì Index (Bảng)
                return RedirectToAction("Choose");
            }
            return View(model);
        }

        // =======================================================
        // 5. CHỈNH SỬA & XÓA (Tất cả đều trả về Choose)
        // =======================================================

        public IActionResult Edit(int id)
        {
            var building = _context.Buildings.Find(id);
            if (building == null) return NotFound();
            return View(building);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Building model)
        {
            if (ModelState.IsValid)
            {
                _context.Buildings.Update(model);
                _context.SaveChanges();
                TempData["msg_flash"] = "Cập nhật thành công!";

                // ĐỔI DÒNG NÀY: Về Choose cho đồng bộ
                return RedirectToAction("Choose");
            }
            return View(model);
        }

        public IActionResult Delete(int id)
        {
            var building = _context.Buildings.Find(id);
            if (building != null)
            {
                _context.Buildings.Remove(building);
                _context.SaveChanges();
                TempData["msg_flash"] = "Đã xóa tòa nhà!";
            }
            // ĐỔI DÒNG NÀY: Về Choose
            return RedirectToAction("Choose");
        }
    }
}