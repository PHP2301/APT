    //using Microsoft.AspNetCore.Mvc;
    //using Microsoft.AspNetCore.Http;
    //using Microsoft.EntityFrameworkCore;
    //using APT.Data;
    //using APT.Models;
    //using System;
    //using System.IO;
    //using System.Linq;

    //namespace APT.Controllers
    //{
    //    public class ManagersController : Controller
    //    {
    //        private readonly ApplicationDbContext _context;

    //        public ManagersController(ApplicationDbContext context)
    //        {
    //            _context = context;
    //        }

    //        // ==============================
    //        // 1. DANH SÁCH MANAGER
    //        // GET: /managers
    //        // ==============================
    //        public IActionResult Index()
    //        {
    //            if (HttpContext.Session.GetString("role") != "admin")
    //                return RedirectToAction("Login", "Users");

    //            var managers = _context.Users
    //                .Where(u => u.Role == "manager")
    //                .ToList();

    //            return View(managers);
    //        }

    //        // ======================================
    //        // 2. PHÂN CÔNG MANAGER CHO TÒA NHÀ (GET)
    //        // GET: /managers/assign/{buildingId}
    //        // ======================================
    //        public IActionResult Assign(int buildingId)
    //        {
    //            if (HttpContext.Session.GetString("role") != "admin")
    //                return RedirectToAction("Login", "Users");

    //            var building = _context.Buildings.Find(buildingId);
    //            if (building == null) return NotFound("Tòa nhà không tồn tại");

    //            var managers = _context.Users
    //                .Where(u => u.Role == "manager")
    //                .ToList();

    //            var vm = new ManagerAssignViewModel
    //            {
    //                Building = building,
    //                Managers = managers
    //            };

    //            return View(vm);
    //        }

    //        // ======================================
    //        // 2. PHÂN CÔNG / GỠ / TẠO MỚI (POST)
    //        // ======================================
    //        [HttpPost]
    //        [ValidateAntiForgeryToken]
    //        public IActionResult Assign(
    //            int buildingId,
    //            int? assignId,
    //            int? removeId,
    //            bool createNew,
    //            ManagerCreateViewModel model)
    //        {
    //            if (HttpContext.Session.GetString("role") != "admin")
    //                return RedirectToAction("Login", "Users");

    //            // GÁN MANAGER
    //            if (assignId.HasValue)
    //            {
    //                _context.BuildingManagers.Add(new BuildingManager
    //                {
    //                    BuildingId = buildingId,
    //                    ManagerId = assignId.Value
    //                });
    //                _context.SaveChanges();
    //                TempData["msg_flash"] = "Đã thêm quản lý vào tòa nhà!";
    //            }

    //            // GỠ MANAGER
    //            if (removeId.HasValue)
    //            {
    //                var link = _context.BuildingManagers
    //                    .FirstOrDefault(x => x.BuildingId == buildingId && x.ManagerId == removeId.Value);

    //                if (link != null)
    //                {
    //                    _context.BuildingManagers.Remove(link);
    //                    _context.SaveChanges();
    //                    TempData["msg_flash"] = "Đã hủy quyền quản lý!";
    //                }
    //            }

    //            // TẠO MỚI & GÁN
    //            if (createNew)
    //            {
    //                if (string.IsNullOrWhiteSpace(model.FullName) ||
    //                    string.IsNullOrWhiteSpace(model.Phone) ||
    //                    string.IsNullOrWhiteSpace(model.Email))
    //                {
    //                    TempData["msg_flash"] = "Vui lòng nhập đủ thông tin!";
    //                }
    //                else
    //                {
    //                    var exist = _context.Users.FirstOrDefault(u => u.Phone == model.Phone);
    //                    int managerId;

    //                    if (exist != null)
    //                    {
    //                        managerId = exist.Id;
    //                    }
    //                    else
    //                    {
    //                        var salt = Guid.NewGuid().ToString();
    //                        var user = new User
    //                        {
    //                            Name = model.FullName,
    //                            Phone = model.Phone,
    //                            Email = model.Email,
    //                            Role = "manager",
    //                            Salt = salt,
    //                            Password = MD5Helper.Hash("Quanly@123" + salt),
    //                            Avatar = "default_user.png"
    //                        };

    //                        _context.Users.Add(user);
    //                        _context.SaveChanges();
    //                        managerId = user.Id;
    //                    }

    //                    _context.BuildingManagers.Add(new BuildingManager
    //                    {
    //                        BuildingId = buildingId,
    //                        ManagerId = managerId
    //                    });
    //                    _context.SaveChanges();

    //                    TempData["msg_flash"] = "Đã tạo và phân công Manager!";
    //                }
    //            }

    //            return RedirectToAction("Assign", new { buildingId });
    //        }

    //        // ==============================
    //        // 3. IMPORT CSV
    //        // POST: /managers/import
    //        // ==============================
    //        [HttpPost]
    //        [ValidateAntiForgeryToken]
    //        public IActionResult Import(int? buildingId, IFormFile fileCsv)
    //        {
    //            if (HttpContext.Session.GetString("role") != "admin")
    //                return RedirectToAction("Login", "Users");

    //            if (fileCsv == null || fileCsv.Length == 0)
    //            {
    //                TempData["msg_flash"] = "Lỗi file CSV";
    //                return RedirectToAction("Index");
    //            }

    //            using var reader = new StreamReader(fileCsv.OpenReadStream());
    //            reader.ReadLine(); // bỏ header

    //            while (!reader.EndOfStream)
    //            {
    //                var line = reader.ReadLine();
    //                if (string.IsNullOrWhiteSpace(line)) continue;

    //                var row = line.Split(',');
    //                if (row.Length < 3) continue;

    //                var fullname = row[0].Trim();
    //                var phone = row[1].Trim();
    //                var email = row[2].Trim();

    //                var user = _context.Users.FirstOrDefault(u => u.Phone == phone);

    //                if (user == null)
    //                {
    //                    var salt = Guid.NewGuid().ToString();
    //                    user = new User
    //                    {
    //                        Name = fullname,
    //                        Phone = phone,
    //                        Email = email,
    //                        Role = "manager",
    //                        Salt = salt,
    //                        Password = MD5Helper.Hash("Quanly@123" + salt),
    //                        Avatar = "default_user.png"
    //                    };
    //                    _context.Users.Add(user);
    //                    _context.SaveChanges();
    //                }

    //                if (buildingId.HasValue)
    //                {
    //                    bool exists = _context.BuildingManagers.Any(x =>
    //                        x.BuildingId == buildingId.Value && x.ManagerId == user.Id);

    //                    if (!exists)
    //                    {
    //                        _context.BuildingManagers.Add(new BuildingManager
    //                        {
    //                            BuildingId = buildingId.Value,
    //                            ManagerId = user.Id
    //                        });
    //                        _context.SaveChanges();
    //                    }
    //                }
    //            }

    //            TempData["msg_flash"] = "Import Manager thành công!";
    //            return RedirectToAction("Index");
    //        }

    //        // ==============================
    //        // 4. XÓA MANAGER
    //        // POST: /managers/delete/{id}
    //        // ==============================
    //        [HttpPost]
    //        [ValidateAntiForgeryToken]
    //        public IActionResult Delete(int id)
    //        {
    //            var user = _context.Users.Find(id);
    //            if (user != null)
    //            {
    //                _context.Users.Remove(user);
    //                _context.SaveChanges();
    //            }

    //            TempData["msg_flash"] = "Đã xóa Manager!";
    //            return RedirectToAction("Index");
    //        }
    //    }
    //}
