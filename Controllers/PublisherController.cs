using Microsoft.AspNetCore.Mvc;

namespace EduPilot_Web.Controllers
{
    public class PublisherController : Controller
    {
        public IActionResult Login()
        {
            return View("Auth/Login");
        }
        public IActionResult Register()
        {
            return View("Auth/Register");
        }
        
        public IActionResult Index()
        {
            return View("Dashboard/Profile");
        }
        public IActionResult Profile()
        {
            return View("Dashboard/Profile");
        }
        
        public IActionResult AddNewQuestion()
        {
            return View("Dashboard/AddNewQuestion");
        }
    }
}
