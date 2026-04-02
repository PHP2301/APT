using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using APT.Data;
using APT.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace APT.Controllers
{
    // PHẢI dùng : Controller ở đây để tránh vòng lặp chuyển hướng
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================
        // LOGIN
        // ==========================
        public IActionResult Login()
        {
            // Nếu đã login rồi thì vào thẳng Dashboard, không bắt login nữa
            if (HttpContext.Session.GetInt32("user_id") != null)
                return RedirectToAction("Index", "Dashboard");

            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.EmailErr = "Vui lòng nhập đầy đủ thông tin";
                return View();
            }

            // TÌM USER
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user == null)
            {
                ViewBag.EmailErr = "Tài khoản hoặc mật khẩu không chính xác";
                return View();
            }

            // TẠO SESSION
            CreateUserSession(user);

            string role = user.Role?.ToLower() ?? "";

            // ĐIỀU HƯỚNG
            if (role == "admin")
            {
                return RedirectToAction("Index", "Dashboard");
            }  else if(role == "resident")
            {
                return RedirectToAction("User", "Residents");
            }

                return RedirectToAction("Index", "Residents");
        }

        private void CreateUserSession(User user)
        {
            HttpContext.Session.SetInt32("user_id", user.Id);
            HttpContext.Session.SetString("user_email", user.Email ?? "");
            HttpContext.Session.SetString("user_name", user.FullName ?? "Admin");
            HttpContext.Session.SetString("role", user.Role ?? "resident");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // ==========================
        // PROFILE (Vẫn để ở đây, nhưng chỉ truy cập được khi đã có session)
        // ==========================
        // FILE: Controllers/UsersController.cs
        public IActionResult Profile()
        {
            // Lấy ID từ Session
            var user_id = HttpContext.Session.GetInt32("user_id");
            if (user_id == null) return RedirectToAction("Login");

            // Tìm User và trả về View
            var user = _context.Users.Find(user_id);
            return View(user);
        }

        // Xóa bớt một dòng [HttpPost] bị thừa ở đây
        [HttpPost]
        public IActionResult Profile(string fullname, string phone, string email, string id_card, string gender, DateTime? dob)
        {
            var user_id = HttpContext.Session.GetInt32("user_id");
            if (user_id == null) return RedirectToAction("Login");

            var user = _context.Users.Find(user_id);
            if (user != null)
            {
                user.FullName = fullname;
                user.Phone = phone;
                user.Email = email;
                user.IdCard = id_card;
                user.Gender = gender;
                user.Dob = dob;

                _context.SaveChanges();
                HttpContext.Session.SetString("user_name", user.FullName ?? "");
                TempData["msg_flash"] = "Cập nhật hồ sơ thành công!";
            }
            return RedirectToAction("Profile");
        }

        [HttpPost]
        public IActionResult ChangePassword(string current_password, string new_password, string confirm_password)
        {
            int user_id = HttpContext.Session.GetInt32("user_id") ?? 0;
            var user = _context.Users.Find(user_id);

            if (user == null) return RedirectToAction("Login");

            if (current_password != user.Password)
            {
                TempData["msg_flash"] = "Mật khẩu hiện tại không đúng.";
                return RedirectToAction("Profile");
            }

            if (new_password != confirm_password)
            {
                TempData["msg_flash"] = "Mật khẩu mới và xác nhận không khớp.";
                return RedirectToAction("Profile");
            }

            user.Password = new_password;
            _context.SaveChanges();

            TempData["msg_flash"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("Profile");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateProfile(User model)
        {
            // 1. Tìm thằng user gốc trong DB dựa vào ID ẩn truyền lên
            var userInDb = _context.Users.Find(model.Id);

            if (userInDb != null)
            {
                try
                {
                    // 2. Chỉ cập nhật những gì được phép sửa
                    userInDb.FullName = model.FullName;
                    userInDb.Phone = model.Phone;
                    // Email thường dùng để đăng nhập nên Đại Ca cân nhắc cho sửa hay không nhé
                    userInDb.Email = model.Email;

                    // 3. Chốt hạ lưu vào DB
                    _context.SaveChanges();

                    TempData["msg_flash"] = "Cập nhật thông tin cá nhân thành công!";
                }
                catch (Exception ex)
                {
                    TempData["msg_flash"] = "Lỗi: " + ex.Message;
                }
            }

            // Quay lại trang Profile để hưởng thụ thành quả
            return RedirectToAction("Profile");
        }
    }
}