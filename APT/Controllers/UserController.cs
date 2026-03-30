using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using APT.Data;
using APT.Models;
using System.Linq;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace APT.Controllers
{
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
            // Nếu đã login rồi thì vào thẳng Dashboard
            if (HttpContext.Session.GetInt32("user_id") != null)
                return RedirectToAction("Index", "Dashboard");

            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin";
                return View();
            }

            // Dùng Select để tránh lỗi "Invalid column name" nếu Model thừa thuộc tính
            var user = _context.Users
                .Where(u => u.Email == email)
                .Select(u => new {
                    u.Id,
                    u.Email,
                    u.Password,
                    u.FullName,
                    u.Role
                })
                .FirstOrDefault();

            if (user == null)
            {
                ViewBag.EmailErr = "Email không tồn tại";
                return View();
            }

            // Kiểm tra mật khẩu trực tiếp (không mã hóa)
            if (password != user.Password)
            {
                ViewBag.PasswordErr = "Mật khẩu không đúng";
                return View();
            }

            // Đăng nhập thành công -> Tạo Session
            HttpContext.Session.SetInt32("user_id", user.Id);
            HttpContext.Session.SetString("user_email", user.Email ?? "");
            HttpContext.Session.SetString("user_name", user.FullName ?? "");
            HttpContext.Session.SetString("role", user.Role ?? "resident");

            return RedirectToAction("Index", "Dashboard");
        }

        // ==========================
        // LOGOUT
        // ==========================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // ==========================
        // PROFILE
        // ==========================
        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetInt32("user_id");
            if (userId == null)
                return RedirectToAction("Login");

            var user = _context.Users.Find(userId);
            return View(user);
        }

        [HttpPost]
        public IActionResult Profile(string fullname, string phone)
        {
            var userId = HttpContext.Session.GetInt32("user_id");
            if (userId == null)
                return RedirectToAction("Login");

            var user = _context.Users.Find(userId);
            if (user == null)
                return RedirectToAction("Login");

            user.FullName = fullname;
            user.Phone = phone;

            _context.SaveChanges();

            // Cập nhật lại tên hiển thị trong Session
            HttpContext.Session.SetString("user_name", user.FullName ?? "");

            TempData["msg_flash"] = "Cập nhật hồ sơ thành công!";
            return RedirectToAction("Profile");
        }

        // ==========================
        // CHANGE PASSWORD
        // ==========================
        [HttpPost]
        public IActionResult ChangePassword(string current_password, string new_password, string confirm_password)
        {
            var userId = HttpContext.Session.GetInt32("user_id");
            if (userId == null)
                return RedirectToAction("Login");

            if (new_password != confirm_password)
            {
                TempData["msg_flash"] = "Mật khẩu xác nhận không khớp.";
                return RedirectToAction("Profile");
            }

            if (string.IsNullOrEmpty(new_password) || new_password.Length < 6)
            {
                TempData["msg_flash"] = "Mật khẩu mới phải có ít nhất 6 ký tự.";
                return RedirectToAction("Profile");
            }

            var user = _context.Users.Find(userId);
            if (user == null)
                return RedirectToAction("Login");

            // Kiểm tra mật khẩu hiện tại trực tiếp (không mã hóa)
            if (current_password != user.Password)
            {
                TempData["msg_flash"] = "Mật khẩu hiện tại không đúng.";
                return RedirectToAction("Profile");
            }

            // Cập nhật mật khẩu mới trực tiếp
            user.Password = new_password;

            _context.SaveChanges();
            TempData["msg_flash"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("Profile");
        }
    }
}