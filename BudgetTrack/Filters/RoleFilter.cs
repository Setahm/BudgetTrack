using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BudgetTrack.Filters
{
    public class RoleFilter : ActionFilterAttribute
    {
        private readonly string _role;

        public RoleFilter(string role)
        {
            _role = role;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;

            var userRole = httpContext.Session.GetString("UserRole");

            // إذا ما فيه جلسة أو الدور غير مطابق يرجّع المستخدم
            if (string.IsNullOrEmpty(userRole) || userRole != _role)
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
            }

            base.OnActionExecuting(context);
        }
    }
}