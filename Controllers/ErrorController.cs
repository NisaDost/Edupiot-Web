using Microsoft.AspNetCore.Mvc;

namespace EduPilot_Web.Controllers
{
    public class ErrorController : ApiControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult UnderDevelopment()
        {
            return View();
        }
    }
}
