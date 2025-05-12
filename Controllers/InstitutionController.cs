using EduPilot_Web.Filters;
using EduPilot_Web.Models;
using EduPilot_Web.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

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

        private async Task<InstitutionDTO?> GetInstitutionAsync(string institutionId)
        {
            var client = GetApiClient();
            var response = await client.GetAsync($"institution/{institutionId}");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<InstitutionDTO>();
        }

        private async Task<List<StudentDTO>?> GetStudentListAsync(string institutionId)
        {
            var client = GetApiClient();
            var response = await client.GetAsync($"institution/{institutionId}/students");
            if (!response.IsSuccessStatusCode) return new List<StudentDTO>();
            return await response.Content.ReadFromJsonAsync<List<StudentDTO>>();
        }

        private async Task<List<SupervisorDTO>?> GetSupervisorListAsync(string institutionId)
        {
            var client = GetApiClient();
            var response = await client.GetAsync($"institution/{institutionId}/supervisor");
            if (!response.IsSuccessStatusCode) return new List<SupervisorDTO>();
            return await response.Content.ReadFromJsonAsync<List<SupervisorDTO>>();
        }

        [AuthorizeUser(Role = "Institution")]
        public async Task<IActionResult> Profile()
        {
            var institutionId = GetLoggedinInstitutionId();
            var institution = await GetInstitutionAsync(institutionId);

            if (institution == null) return NotFound();

            var students = await GetStudentListAsync(institutionId) ?? new List<StudentDTO>();
            var supervisors = await GetSupervisorListAsync(institutionId) ?? new List<SupervisorDTO>();

            var model = new InstitutionViewModel
            {
                Id = institution.Id,
                Name = institution.Name,
                Email = institution.Email,
                Address = institution.Address,
                Website = institution.Website,
                Logo = institution.Logo,
                Students = students,
                Supervisors = supervisors
            };

            return View("Dashboard/Profile", model);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] PublisherViewModel updated)
        {
            var institutionId = GetLoggedinInstitutionId();
            var current = await GetInstitutionAsync(institutionId);
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

        [HttpGet]
        public async Task<IActionResult> StudentList()
        {
            var institutionId = GetLoggedinInstitutionId();
            var studentList = await GetStudentListAsync(institutionId);
            if (studentList == null) return NotFound();

            return View("Dashboard/Student/StudentList", studentList);
        }

        [AllowAnonymous]
        public IActionResult Login() => View("Auth/Login");

        [AllowAnonymous]
        public IActionResult Register() => View("Auth/Register");

        public IActionResult Index() => View("Dashboard/Profile");

        public IActionResult ClassList() => View("Dashboard/Student/ClassList");

        public IActionResult TeacherList() => View("Dashboard/Teacher/TeacherList");

        public IActionResult Attendance() => View("Dashboard/Student/Attendance");

        public IActionResult CurrentPlan() => View("Dashboard/CurrentPlan");
    }
}
