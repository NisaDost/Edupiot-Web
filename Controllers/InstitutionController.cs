using EduPilot_Web.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EduPilot_Web.Controllers
{
    public class InstitutionController : ApiControllerBase
    {

        public InstitutionController(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }

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
        public IActionResult StudentList()
        {
            return View("Dashboard/Student/StudentList");
        }

        public IActionResult ClassList()
        {
            return View("Dashboard/Student/ClassList");
        }

        public IActionResult TeacherList()
        {
            return View("Dashboard/Teacher/TeacherList");
        }

        public IActionResult Attendance()
        {
            return View("Dashboard/Student/Attendance");
        }

        public IActionResult CurrentPlan()
        {
            return View("Dashboard/CurrentPlan");
        }
    }
}
