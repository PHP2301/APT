using Microsoft.AspNetCore.Mvc;
using APT.Models;

namespace APT.Controllers
{
    public class LoginController : Controller
    {
        // ADMIN HARD CODE
        private const string ADMIN_EMAIL = "admin@gmail.com";
        private const string ADMIN_PASSWORD = "123456";

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string email, string password)
        {
            // kiểm tra tài khoản admin cứng
            if (email == ADMIN_EMAIL && password == ADMIN_PASSWORD)
            {
                // lưu session đăng nhập
                HttpContext.Session.SetString("User", "admin");
                HttpContext.Session.SetString("Role", "admin");

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Sai tài khoản hoặc mật khẩu!";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}