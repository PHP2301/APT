using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using APT.Data;
using APT.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
        // GET + POST: /users/login
        // ==========================
        public IActionResult Login()
        {
            // Đã login → đá về dashboard
            if (HttpContext.Session.GetInt32("user_id") != null)
                return RedirectToAction("Index", "Dashboard");

            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ViewBag.EmailErr = "Vui lòng nhập email";
                return View();
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ViewBag.PasswordErr = "Vui lòng nhập mật khẩu";
                return View();
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                ViewBag.EmailErr = "Email không tồn tại";
                return View();
            }

            //// ✅ chỉ BCrypt
            //if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            //{
            //    ViewBag.PasswordErr = "Mật khẩu không đúng";
            //    return View();
            //}

            CreateUserSession(user);
            return RedirectToAction("Index", "Dashboard");
        }

        // ==========================
        // CREATE SESSION
        // ==========================
        private void CreateUserSession(User user)
        {
            HttpContext.Session.SetInt32("user_id", user.Id);
            HttpContext.Session.SetString("user_email", user.Email);
            HttpContext.Session.SetString("user_name", user.FullName ?? "");
            HttpContext.Session.SetString("role", user.Role);

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
        // GET + POST: /users/profile
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
            HttpContext.Session.SetString("user_name", user.FullName ?? "");

            TempData["msg_flash"] = "Cập nhật hồ sơ thành công!";
            return RedirectToAction("Profile");
        }

        // ==========================
        // CHANGE PASSWORD
        // POST: /users/change_password
        // ==========================
        [HttpPost]
        public IActionResult ChangePassword(
    string current_password,
    string new_password,
    string confirm_password)
        {
            var userId = HttpContext.Session.GetInt32("user_id");
            if (userId == null)
                return RedirectToAction("Login");

            if (new_password != confirm_password)
            {
                TempData["msg_flash"] = "Mật khẩu xác nhận không khớp.";
                return RedirectToAction("Profile");
            }

            if (new_password.Length < 6)
            {
                TempData["msg_flash"] = "Mật khẩu mới phải có ít nhất 6 ký tự.";
                return RedirectToAction("Profile");
            }

            var user = _context.Users.Find(userId);
            if (user == null)
                return RedirectToAction("Login");

            // ✅ kiểm tra mật khẩu cũ bằng BCrypt
            if (!BCrypt.Net.BCrypt.Verify(current_password, user.Password))
            {
                TempData["msg_flash"] = "Mật khẩu hiện tại không đúng.";
                return RedirectToAction("Profile");
            }

            // ✅ lưu mật khẩu mới
            user.Password = BCrypt.Net.BCrypt.HashPassword(new_password);

            _context.SaveChanges();

            TempData["msg_flash"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("Profile");
        }
        // ==========================
        // MD5 HELPER
        // ==========================

    }
}
