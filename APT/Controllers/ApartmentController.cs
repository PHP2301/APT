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
    public class ApartmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApartmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================
        // 1. DANH SÁCH CĂN HỘ
        // ==========================
        public IActionResult Index(int? building_id) // Đổi buildingId -> building_id
        {
            if (!building_id.HasValue || building_id == 0)
            {
                var firstBuilding = _context.Buildings.OrderBy(b => b.Id).FirstOrDefault();
                building_id = firstBuilding?.Id ?? 0;
            }

            if (building_id == 0)
            {
                TempData["msg_flash"] = "Vui lòng tạo ít nhất một tòa nhà trước!";
                return RedirectToAction("Create", "Buildings");
            }

            // MẸO QUAN TRỌNG: Lưu vào Session để ResidentsController có thể đọc được
            HttpContext.Session.SetInt32("current_building_id", building_id.Value);

            var apartments = (from a in _context.Apartments
                              join b in _context.Buildings on a.building_id equals b.Id
                              join u in _context.Users on a.ResidentId equals u.Id into userGroup
                              from res in userGroup.DefaultIfEmpty()
                              where a.building_id == building_id
                              select new Apartment
                              {
                                  Id = a.Id,
                                  FullRoomName = a.FullRoomName,
                                  Building = b,
                                  Resident = res,
                                  building_id = a.building_id,
                                  Status = a.Status,
                                  RentPrice = a.RentPrice,
                                  FloorNumber = a.FloorNumber,
                                  RoomType = a.RoomType
                              }).ToList();

            ViewBag.Buildings = _context.Buildings.ToList();
            ViewBag.building_id = building_id; // Đảm bảo đúng tên để View nhận
            ViewBag.CurrentBuildingName = _context.Buildings.Find(building_id)?.Name;

            return View(apartments);
        }

        // ==========================
        // 2. CHỈNH SỬA CĂN HỘ (EDIT)
        // ==========================
        public IActionResult Edit(int id)
        {
            var apt = _context.Apartments.Find(id);
            if (apt == null) return NotFound();

            ViewBag.Buildings = _context.Buildings.ToList();
            return View(apt);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Apartment model)
        {
            // 1. Tìm đối tượng GỐC trong Database trước
            var aptInDb = _context.Apartments.Find(model.Id);

            if (aptInDb == null)
            {
                TempData["msg_flash"] = "Lỗi: Không tìm thấy căn hộ!";
                return RedirectToAction("Index");
            }

            // 2. Chỉ cập nhật những trường Đại Ca thay đổi trên Form
            aptInDb.RoomType = model.RoomType;
            aptInDb.Area = model.Area;
            aptInDb.RentPrice = model.RentPrice;
            aptInDb.Status = model.Status;

            // Đảm bảo building_id không bị lạc trôi
            if (model.building_id != 0)
            {
                aptInDb.building_id = model.building_id;
            }

            try
            {
                // 3. Lệnh chốt hạ để lưu xuống SQL
                _context.SaveChanges();

                TempData["msg_flash"] = "Đại Ca đã cập nhật căn hộ thành công!";
                return RedirectToAction("Index", new { building_id = aptInDb.building_id });
            }
            catch (Exception ex)
            {
                // Nếu có lỗi (ví dụ sai kiểu dữ liệu), nó sẽ báo ở đây
                ModelState.AddModelError("", "Lỗi hệ thống: " + ex.Message);
                ViewBag.Buildings = _context.Buildings.ToList();
                return View(model);
            }
        }

        // ==========================
        // 3. XÓA CĂN HỘ (DELETE)
        // ==========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            // Chỉ tìm Apartment, KHÔNG Include Vehicles nữa
            var apt = _context.Apartments.FirstOrDefault(a => a.Id == id);

            if (apt != null)
            {
                int bId = apt.building_id;

                // --- ĐÃ XÓA LOGIC DỌN DẸP XE ---

                _context.Apartments.Remove(apt);
                _context.SaveChanges();
                TempData["msg_flash"] = "Đã xóa căn hộ thành công.";
                return RedirectToAction("Index", new { buildingId = bId });
            }
            return RedirectToAction("Index");
        }

        // ==========================
        // 4. TẠO PHÒNG HÀNG LOẠT (BATCH)
        // ==========================
        public IActionResult CreateBatch()
        {
            ViewBag.Buildings = _context.Buildings.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ProcessCreateBatch(int BuildingId, int FromFloor, int ToFloor, int RoomsPerFloor, string Type, decimal Area, Dictionary<int, decimal> FloorPrices)
        {
            try
            {
                for (int floor = FromFloor; floor <= ToFloor; floor++)
                {
                    decimal price = (FloorPrices != null && FloorPrices.ContainsKey(floor)) ? FloorPrices[floor] : 0;

                    for (int roomIndex = 1; roomIndex <= RoomsPerFloor; roomIndex++)
                    {
                        string suffix = roomIndex < 10 ? "0" + roomIndex : roomIndex.ToString();
                        string fullRoomName = $"{floor}{suffix}";

                        var newRoom = new Apartment
                        {
                            building_id = BuildingId,
                            FloorNumber = floor,
                            RoomType = Type,
                            RoomNumberSuffix = suffix,
                            FullRoomName = fullRoomName,
                            Area = Area,
                            RentPrice = price,
                            Status = "Available",
                            ManagementPrice = 0
                        };

                        _context.Apartments.Add(newRoom);
                    }
                }

                _context.SaveChanges();
                TempData["msg_flash"] = "Đã tạo phòng tự động thành công!";
                return RedirectToAction("Index", new { buildingId = BuildingId });
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi: " + ex.Message;
                ViewBag.Buildings = _context.Buildings.ToList();
                return View("CreateBatch");
            }
        }
    }
}