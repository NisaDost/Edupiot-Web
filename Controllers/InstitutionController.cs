using EduPilot_Web.Filters;
using EduPilot_Web.Models;
using EduPilot_Web.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduPilot_Web.Controllers
{
    public class InstitutionController : ApiControllerBase
    {

        public InstitutionController(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }
        public string GetLoggedinInstitutionId()
        {
            var institutionId = HttpContext.Session.GetString("InstitutionId")?.Trim('"');
            return institutionId ?? string.Empty;
        }

        private InstitutionDTO? GetInstitution(string institutionId)
        {
            var client = GetApiClient();
            var response = client.GetAsync($"institution/{institutionId}").Result;
            if (!response.IsSuccessStatusCode) return null;

            return response.Content.ReadFromJsonAsync<InstitutionDTO>().Result;
        }

        [AuthorizeUser(Role = "Institution")]
        public IActionResult Profile()
        {
            var institutionId = GetLoggedinInstitutionId();
            var institution = GetInstitution(institutionId);

            if (institution == null)
            {
                return NotFound();
            }

            var model = new InstitutionViewModel
            {
                Id = institution.Id,
                Name = institution.Name,
                Email = institution.Email,
                Address = institution.Address,
                Website = institution.Website,
                Logo = institution.Logo,

                //burayı ayrıca mı çekeceğime karar vereceğim,
                //hem listeye hem liste uzunluğuna ihtiyaç var
                Students = new List<StudentViewModel>(),
                Supervisors = new List<SupervisorViewModel>(),
            };

            return View("Dashboard/Profile", model);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] PublisherViewModel updated)
        {
            var institutionId = GetLoggedinInstitutionId();
            var current = GetInstitution(institutionId);
            if (current == null) return BadRequest("Geçerli kullanıcı bulunamadı.");


            var updatedInfo = new
            {
                name = string.IsNullOrWhiteSpace(updated.Name) ? current.Name : updated.Name,
                email = current.Email,
                address = string.IsNullOrWhiteSpace(updated.Address) ? current.Address : updated.Address,
                website = string.IsNullOrWhiteSpace(updated.Website) ? current.Website : updated.Website,
                logo = string.IsNullOrWhiteSpace(updated.Logo) ? current.Logo : updated.Logo,
                currentPassword = string.IsNullOrWhiteSpace(updated.CurrentPassword) ? current.CurrentPassword : updated.CurrentPassword,
                password = string.IsNullOrWhiteSpace(updated.NewPassword) ? current.NewPassword : updated.NewPassword
            };

            var client = GetApiClient();
            var response = await client.PutAsJsonAsync($"institution/{institutionId}", updatedInfo);

            return response.IsSuccessStatusCode ? Ok() : BadRequest("Profil güncellenemedi.");
        }



        [AllowAnonymous]
        public IActionResult Login()
        {
            return View("Auth/Login");
        }
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View("Auth/Register");
        }

        public IActionResult Index()
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
