using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EduPilot.Web.Filters
{
    public class AuthorizeUserAttribute : ActionFilterAttribute
    {
        public string Role { get; set; } // "Publisher" veya "Institution"

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var http = context.HttpContext;
            var userType = http.Session.GetString("UserType");
            var controller = context.RouteData.Values["controller"]?.ToString();
            var action = context.RouteData.Values["action"]?.ToString();

            // Eğer login/register sayfalarıysa kontrol etme
            if (action is "Login" or "Register")
            {
                base.OnActionExecuting(context);
                return;
            }

            // UserType varsa ama ilgili Id yoksa (örneğin session düşmüşse)
            bool isPublisher = userType == "Publisher";
            bool isInstitution = userType == "Institution";

            var hasPublisherId = !string.IsNullOrEmpty(http.Session.GetString("PublisherId"));
            var hasInstitutionId = !string.IsNullOrEmpty(http.Session.GetString("InstitutionId"));

            bool isAuthorized = (isPublisher && hasPublisherId && Role == "Publisher")
                             || (isInstitution && hasInstitutionId && Role == "Institution");

            if (!isAuthorized)
            {
                var redirectController = Role == "Publisher" ? "Publisher" : "Institution";
                context.Result = new RedirectToActionResult("Login", redirectController, null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
