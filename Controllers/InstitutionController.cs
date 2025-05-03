using EduPilot_Web.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EduPilot_Web.Controllers
{
    public class InstitutionController : ApiControllerBase
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View("Auth/Login");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View("Auth/Register");
        }

        [HttpPost]
        public async Task<IActionResult> RegisterSubmit(InstitutionRegisterDTO model)
        {
            if (!ModelState.IsValid) return View("Auth/Register", model);

            var response = await _client.PostAsJsonAsync("/api/institution/register", model);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login");
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"API Error: {errorContent}");
            return View("Auth/Register", model);
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
