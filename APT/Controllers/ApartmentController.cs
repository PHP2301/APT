using APT.Data;
using APT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace APT.Controllers
{
    public class ApartmentsController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public ApartmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================
        // DANH SÁCH CĂN HỘ
        // ==========================
        public IActionResult Index(int? buildingId)
        {
            var query = _context.Apartments
                .Include(a => a.Building)
                .Include(a => a.Resident)
                .AsQueryable();

            if (buildingId.HasValue)
                query = query.Where(a => a.building_id == buildingId);

            ViewBag.Buildings = _context.Buildings.ToList();
            ViewBag.CurrentBuildingId = buildingId;

            return View(query.ToList());
        }

        // ==========================
        // CHỈNH SỬA CĂN HỘ (EDIT)
        // ==========================
        public IActionResult Edit(int id)
        {
            var apt = _context.Apartments.Find(id);
            if (apt == null) return NotFound();

            ViewBag.Buildings = _context.Buildings.ToList();
            return View(apt);
        }

        [HttpPost]
        public IActionResult Edit(Apartment model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Buildings = _context.Buildings.ToList();
                return View(model);
            }

            _context.Apartments.Update(model);
            _context.SaveChanges();

            TempData["msg_flash"] = "Cập nhật căn hộ thành công!";
            return RedirectToAction("Index", new { buildingId = model.building_id });
        }

        // ==========================
        // XÓA CĂN HỘ (DELETE) - Mới thêm
        // ==========================
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var apt = _context.Apartments.Find(id);
            if (apt != null)
            {
                int? bId = apt.building_id;
                _context.Apartments.Remove(apt);
                _context.SaveChanges();
                TempData["msg_flash"] = "Đã xóa căn hộ.";
                return RedirectToAction("Index", new { buildingId = bId });
            }
            return RedirectToAction("Index");
        }

        // ==========================
        // TẠO PHÒNG HÀNG LOẠT (BATCH)
        // ==========================
        public IActionResult CreateBatch()
        {
            ViewBag.Buildings = _context.Buildings.ToList();
            return View();
        }

        [HttpPost]
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