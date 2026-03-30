using APT.Data;
using APT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APT.Controllers
{
    public class ApartmentsController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public ApartmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? buildingId)
        {
            var query = _context.Apartments
                .Include(a => a.Building)
                .Include(a => a.Resident)
                .AsQueryable();

            if (buildingId.HasValue)
                query = query.Where(a => a.Building_id == buildingId);

            ViewBag.Buildings = _context.Buildings.ToList();

            return View(query.ToList());
        }

        public IActionResult Edit(int id)
        {
            var apt = _context.Apartments.Find(id);

            if (apt == null)
                return NotFound();

            return View(apt);
        }

        [HttpPost]
        public IActionResult Edit(Apartment model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.Apartments.Update(model);
            _context.SaveChanges();

            return RedirectToAction("Index",
                new { buildingId = model.Building_id });
        }
    }
}