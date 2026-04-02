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
            var apartment = _context.Apartments
                .Include(a => a.Resident)
                .FirstOrDefault(a => a.Id == roomId);

            if (apartment == null) return NotFound();

            // CỰC KỲ QUAN TRỌNG: Phải nạp LatestBill vào đây thì View mới thấy
            ViewBag.LatestBill = _context.Bills
                .Where(b => b.RoomId == roomId && b.Status == false)
                .OrderByDescending(b => b.CreatedAt)
                .FirstOrDefault();

            return View(apartment);
        }

        // 4. POST: Xác nhận thu tiền qua Ajax
        // --- DÀNH CHO ADMIN ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AdminConfirmPayment(int apartmentId, decimal amount)
        {
            // Tìm bill chưa thanh toán của phòng này
            var bill = _context.Bills
                .Where(b => b.RoomId == apartmentId && b.Status == false)
                .OrderByDescending(b => b.CreatedAt)
                .FirstOrDefault();

            if (bill == null)
            {
                // Nếu không có bill nợ, tạo mới hoàn toàn
                bill = new Bill
                {
                    RoomId = apartmentId,
                    TotalMoney = (int)amount,
                    Status = false, // Admin thu tiền mặt thì chốt luôn là 1
                    CreatedAt = DateTime.Now,
                    PaidAt = DateTime.Now,
                    Month = "Tháng " + DateTime.Now.Month
                };
                _context.Bills.Add(bill);
            }
            else
            {
                // Nếu đã có bill nợ (do Admin bấm chốt nước trước đó), thì cập nhật nó thành Đã thanh toán
                bill.Status = true;
                bill.PaidAt = DateTime.Now;
                bill.TotalMoney = (int)amount;
            }

            _context.SaveChanges();
            return Json(new { success = true, message = "Đã Nhận Tiền!" });
        }

        // --- DÀNH CHO CƯ DÂN ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResidentProcessPayment(int apartmentId, decimal amount)
        {
            // Cư dân chỉ được thanh toán HÓA ĐƠN ĐÃ CÓ SẴN (do Admin chốt số nước rồi)
            var bill = _context.Bills
                .Where(b => b.RoomId == apartmentId && b.Status == false)
                .OrderByDescending(b => b.CreatedAt)
                .FirstOrDefault();

            if (bill == null)
            {
                return Json(new { success = false, message = "Hóa đơn chưa được Ban quản lý chốt số nước. Đại Ca vui lòng đợi Admin nhé!" });
            }

            // Chốt trạng thái thanh toán online
            bill.Status = true;
            bill.PaidAt = DateTime.Now;
            _context.SaveChanges();

            return Json(new { success = true, message = "Đại Ca đã thanh toán hóa đơn thành công rực rỡ!" });
        }
    }
}