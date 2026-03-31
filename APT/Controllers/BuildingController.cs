using APT.Data;
using APT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APT.Controllers
{
    public class BuildingsController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public BuildingsController(ApplicationDbContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            return View(_context.Buildings.ToList());
        }

       


        [HttpPost]
        public IActionResult Create(Building model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.Buildings.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        public IActionResult Edit(int id)
        {
            // Tìm tòa nhà theo ID
            var building = _context.Buildings.Find(id);

            if (building == null)
            {
                return NotFound();
            }

            // Trả về View Edit cùng dữ liệu của tòa nhà đó
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
                    return RedirectToAction("Index"); // Lưu xong bay về trang danh sách
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi lưu dữ liệu: " + ex.Message);
                }
            }
            // Nếu có lỗi (dữ liệu không hợp lệ), trả về View cùng dữ liệu đã nhập để sửa lại
            return View(model);
        }

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