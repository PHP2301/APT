using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

namespace APT.Controllers
{
    public class BaseController : Controller
    {
        // Kiểm tra xem user đã login chưa
        protected bool IsLoggedIn => HttpContext.Session.GetInt32("user_id") != null;
        protected string? UserRole => HttpContext.Session.GetString("role");

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Nếu chưa login, đá về trang Login của UsersController
            if (!IsLoggedIn)
            {
                context.Result = new RedirectToActionResult("Login", "Users", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}