using APT.Data;
using APT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace APT.Controllers
{
    public class BillingController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public BillingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Danh sách hóa đơn
        public IActionResult Index()
        {
            var bills = _context.Bills
                .Include(b => b.Apartment)
                .OrderByDescending(b => b.CreatedAt)
                .ToList();

            return View(bills);
        }

        // 2. Tạo hóa đơn tháng (ĐÃ SỬA THEO CÁCH 2)
        public IActionResult GenerateMonthly(string month)
        {
            var apartments = _context.Apartments
                .Include(a => a.Resident)
                .Include(a => a.ApartmentServices)
                .ToList();

            foreach (var apt in apartments)
            {
                bool exists = _context.Bills.Any(b => b.RoomId == apt.Id && b.Month == month);
                if (exists) continue;

                // SỬA: Dùng decimal để tính toán cho chính xác
                decimal total = 0;

                // Tiền thuê & Phí quản lý (Dùng ?? 0 để tránh lỗi Null)
                total += apt.RentPrice ?? 0;
                total += apt.ManagementPrice ?? 0;

                // Tiền điện nước
                var reading = _context.UtilityReadings
                    .FirstOrDefault(u => u.ApartmentId == apt.Id && u.Month == month);

                if (reading != null)
                {
                    int elecUsage = reading.ElecNew - reading.ElecOld;
                    int waterUsage = reading.WaterNew - reading.WaterOld;

                    // Cộng dồn vào biến decimal
                    total += (elecUsage * 3500) + (waterUsage * 7000);
                }

                // Tiền dịch vụ
                foreach (var s in apt.ApartmentServices)
                {
                    var service = _context.Services.FirstOrDefault(x => x.Id == s.ServiceId);
                    if (service != null)
                    {
                        // Cộng dồn decimal, không bị lỗi cast
                        total += service.Price;
                    }
                }

                // Tạo đối tượng Bill
                Bill bill = new Bill
                {
                    RoomId = apt.Id,
                    Month = month,
                    // ÉP KIỂU VỀ INT Ở BƯỚC CUỐI CÙNG ĐỂ LƯU VÀO DB
                    TotalMoney = (int)total,
                    Status = false,
                    CreatedAt = DateTime.Now
                };
                _context.Bills.Add(bill);
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // 3. GET: Chi tiết phòng để lập hóa đơn
        public IActionResult RoomDetails(int roomId)
        {
            var apt = _context.Apartments
                .Include(a => a.Building)
                .Include(a => a.Resident)
                    .ThenInclude(r => r.Vehicles)
                .Include(a => a.ApartmentServices)
                    .ThenInclude(asv => asv.Service)
                .FirstOrDefault(a => a.Id == roomId);

            if (apt == null) return NotFound();

            var latestReading = _context.UtilityReadings
                .Where(u => u.ApartmentId == roomId)
                .OrderByDescending(u => u.Id)
                .FirstOrDefault();

            ViewBag.Reading = latestReading;

            return View(apt);
        }

        // 4. POST: Xác nhận thu tiền qua Ajax
        [HttpPost]
        public IActionResult ConfirmPayment(int apartmentId, decimal amount)
        {
            var bill = _context.Bills
                .Where(b => b.RoomId == apartmentId && b.Status == false)
                .OrderByDescending(b => b.CreatedAt)
                .FirstOrDefault();

            if (bill != null)
            {
                bill.Status = true;
                bill.PaidAt = DateTime.Now;
                _context.SaveChanges();

                return Json(new { success = true, message = "Đã thu tiền thành công!" });
            }

            return Json(new { success = false, message = "Không tìm thấy hóa đơn!" });
        }
    }
}