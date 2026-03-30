using APT.Data;
using APT.Models;
using Microsoft.AspNetCore.Mvc;

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

        public IActionResult Create()
        {
            return View();
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