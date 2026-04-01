using APT.Data;
using APT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        // 1. Trang danh sách tòa nhà (Cho nút "Quản lý tòa nhà" ngoài Dashboard)
        public IActionResult Index()
        {
            // Include Basements nếu Đại Ca muốn đếm số hầm ở View Index
            var list = _context.Buildings.Include(b => b.Basements).ToList();
            return View(list);
        }

        // 2. Action quan trọng nhất: Cho Admin chọn tòa nhà trước khi xem Căn hộ
        // Fix lỗi "Truy cập" từ Admin Dashboard
        public IActionResult ChooseBuilding()
        {
            // Lấy danh sách tòa nhà kèm theo số hầm để hiển thị Card xịn xò
            var buildings = _context.Buildings.Include(b => b.Basements).ToList();

            // Trỏ đúng về file Views/Buildings/ChooseBuilding.cshtml
            return View(buildings);
        }

        // 3. Chỉnh sửa tòa nhà
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
                try
                {
                    _context.Buildings.Update(model);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi lưu dữ liệu: " + ex.Message);
                }
            }
            return View(model);
        }

        // 4. Tạo mới tòa nhà
        [HttpPost]
        public IActionResult Create(Building model)
        {
            if (!ModelState.IsValid) return View("Index", _context.Buildings.ToList());

            _context.Buildings.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // 5. Xóa tòa nhà
        public IActionResult Delete(int id)
        {
            var building = _context.Buildings.Find(id);
            if (building != null)
            {
                _context.Buildings.Remove(building);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}