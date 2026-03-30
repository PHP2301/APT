using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace APT.Controllers
{
    public class AdminController : BaseController
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (Role != "admin")
            {
                context.Result = new RedirectToActionResult("Login", "Users", null);
                return;
            }

            base.OnActionExecuting(context);
        }

        public IActionResult Dashboard()
        {
            ViewData["Title"] = "Admin Dashboard";
            return View();
        }
    }
}