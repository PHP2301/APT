using APT.Data;
using APT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APT.Controllers
{
    public class ResidentsController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public ResidentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int buildingId)
        {
            var residents = _context.Residents
                .Where(r => r.BuildingId == buildingId)
                .Include(r => r.User)
                .ToList();

            return View(residents);
        }

        [HttpPost]
        public IActionResult Add(Resident model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index",
                    new { buildingId = model.BuildingId });

            _context.Residents.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Index",
                new { buildingId = model.BuildingId });
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var resident = _context.Residents.Find(id);

            if (resident != null)
            {
                _context.Residents.Remove(resident);
                _context.SaveChanges();
            }

            return RedirectToAction("Index",
                new { buildingId = resident?.BuildingId });
        }
    }
}