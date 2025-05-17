using Microsoft.AspNetCore.Mvc;

namespace EduPilot.Web.Controllers
{
    public class ErrorController : ApiControllerBase
    {

        public ErrorController(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }
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
