using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace APT.Controllers
{
    public class BaseController : Controller
    {
        protected string Role => HttpContext.Session.GetString("role");

        protected bool IsLoggedIn =>
            HttpContext.Session.GetInt32("user_id") != null;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!IsLoggedIn)
            {
                context.Result = new RedirectToActionResult("Login", "Users", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}