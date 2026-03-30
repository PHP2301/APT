using APT.Data;
using APT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APT.Controllers
{
    public class BillingController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public BillingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // danh sách hóa đơn
        public IActionResult Index()
        {
            var bills = _context.Bills
                .Include(b => b.Apartment)
                .OrderByDescending(b => b.CreatedAt)
                .ToList();

            return View(bills);
        }

        // tạo hóa đơn tháng
        public IActionResult GenerateMonthly(string month)
        {
            var apartments = _context.Apartments
                .Include(a => a.Resident)
                .Include(a => a.ApartmentServices)
                .ToList();

            foreach (var apt in apartments)
            {
                bool exists = _context.Bills.Any(b =>
                    b.RoomId == apt.Id &&
                    b.Month == month);

                if (exists)
                    continue;

                int total = 0;

                // tiền thuê
                total += (int)apt.RentPrice;

                // phí quản lý
                total += apt.ManagementPrice;

                // điện nước
                var reading = _context.UtilityReadings
                    .FirstOrDefault(u =>
                        u.ApartmentId == apt.Id &&
                        u.Month == month);

                if (reading != null)
                {
                    int elecUsage = reading.ElecNew - reading.ElecOld;
                    int waterUsage = reading.WaterNew - reading.WaterOld;

                    int elecMoney = elecUsage * 3500;
                    int waterMoney = waterUsage * 7000;

                    total += elecMoney + waterMoney;
                }

                // dịch vụ
                foreach (var s in apt.ApartmentServices)
                {
                    var service = _context.Services
                        .FirstOrDefault(x => x.Id == s.ServiceId);

                    if (service != null)
                        total += (int)service.Price;
                }

                Bill bill = new Bill
                {
                    RoomId = apt.Id,
                    Month = month,
                    TotalMoney = total,
                    Status = false,
                    CreatedAt = DateTime.Now
                };

                _context.Bills.Add(bill);
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}