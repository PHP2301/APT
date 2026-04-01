using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using APT.Data;
using APT.Models;
using System.Linq;
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

            // TÌM USER KHỚP CẢ EMAIL VÀ PASSWORD (PLAIN TEXT)
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user == null)
            {
                // Kiểm tra xem do sai Email hay sai Pass để báo cho chuẩn
                var checkEmail = _context.Users.Any(u => u.Email == email);
                if (!checkEmail)
                {
                    ViewBag.EmailErr = "Email không tồn tại";
                }
                else
                {
                    ViewBag.PasswordErr = "Mật khẩu không chính xác";
                }
                return View();
            }

            // TẠO SESSION VÀ ĐIỀU HƯỚNG
            CreateUserSession(user);

            // Điều hướng dựa trên Role
            string role = user.Role?.ToLower() ?? "";
            if (role == "admin" || role == "super_admin")
            {
                return RedirectToAction("Index", "Dashboard"); // Sẽ vào AdminDashboard trong DashboardController
            }

            return RedirectToAction("Index", "Dashboard"); // Sẽ vào ManagerDashboard hoặc ResidentDashboard
        }

        private void CreateUserSession(User user)
        {
            HttpContext.Session.SetInt32("user_id", user.Id);
            HttpContext.Session.SetString("user_email", user.Email);
            HttpContext.Session.SetString("user_name", user.FullName ?? "");
            HttpContext.Session.SetString("role", user.Role ?? "resident");
        }

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
            var user_id = HttpContext.Session.GetInt32("user_id");
            if (user_id == null) return RedirectToAction("Login");

            var user = _context.Users.Find(user_id);
            return View(user);
        }

        [HttpPost]
        public IActionResult Profile(string fullname, string phone)
        {
            var user_id = HttpContext.Session.GetInt32("user_id");
            if (user_id == null) return RedirectToAction("Login");

            var user = _context.Users.Find(user_id);
            if (user != null)
            {
                user.FullName = fullname;
                user.Phone = phone;
                _context.SaveChanges();
                HttpContext.Session.SetString("user_name", user.FullName ?? "");
                TempData["msg_flash"] = "Cập nhật hồ sơ thành công!";
            }
            return RedirectToAction("Profile");
        }

        // ==========================
        // CHANGE PASSWORD (KHÔNG MÃ HÓA)
        // ==========================
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
    }
}