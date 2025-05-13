using EduPilot_Web.Models.DTOs;
using EduPilot_Web.Models;
using Microsoft.AspNetCore.Mvc;
using EduPilot_Web.Filters;
using Microsoft.AspNetCore.Authorization;

namespace EduPilot_Web.Controllers
{
    [AuthorizeUser(Role = "Publisher")]
    public class PublisherController : ApiControllerBase
    {
        public PublisherController(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }

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

        public string GetLoggedinPublisherId()
        {
            var publisherId = HttpContext.Session.GetString("PublisherId")?.Trim('"');
            return publisherId ?? string.Empty;
        }

        private PublisherDTO? GetPublisher(string publisherId)
        {
            var client = GetApiClient();
            var response = client.GetAsync($"publisher/{publisherId}").Result;
            if (!response.IsSuccessStatusCode) return null;

            return response.Content.ReadFromJsonAsync<PublisherDTO>().Result;
        }

        [AuthorizeUser(Role = "Publisher")]
        public IActionResult Profile()
        {
            var publisherId = GetLoggedinPublisherId();
            var publisher = GetPublisher(publisherId);

            if (publisher == null)
            {
                return NotFound();
            }

            var model = new PublisherViewModel
            {
                Id = publisher.Id,
                Name = publisher.Name,
                Email = publisher.Email,
                Address = publisher.Address,
                Website = publisher.Website,
                Logo = publisher.Logo,
                QuizCount = publisher.quizCount,
                QuestionCount = publisher.questionCount,
            };

            return View("Profile", model);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] PublisherViewModel updated)
        {
            var publisherId = GetLoggedinPublisherId();
            var current = GetPublisher(publisherId);
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
            var response = await client.PutAsJsonAsync($"publisher/{publisherId}", updatedInfo);

            return response.IsSuccessStatusCode ? Ok() : BadRequest("Profil güncellenemedi.");
        }

        public IActionResult CreateQuiz()
        {
            return View("CreateQuiz");
        }

        [HttpGet]
        public async Task<IActionResult> LoadLessons(int grade)
        {
            var client = GetApiClient();
            var response = await client.GetAsync($"lessons/{grade}");

            if (!response.IsSuccessStatusCode)
                return Json(new { success = false, lessons = new List<LessonsByGradeDTO>() });

            var lessons = await response.Content.ReadFromJsonAsync<List<LessonsByGradeDTO>>();
            return Json(new { success = true, lessons });
        }

        [HttpGet]
        public async Task<IActionResult> LoadSubjects(Guid lessonId)
        {
            var client = GetApiClient();
            var response = await client.GetAsync($"lessons/{lessonId}/subjects");

            if (!response.IsSuccessStatusCode)
                return Json(new { success = false, subjects = new List<SubjectDTO>() });

            var subjects = await response.Content.ReadFromJsonAsync<List<SubjectDTO>>();
            return Json(new { success = true, subjects });
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuiz([FromBody] QuizDTO quizDto)
        {
            var publisherId = GetLoggedinPublisherId();
            var client = GetApiClient();

            var res = await client.PostAsJsonAsync($"publisher/{publisherId}/add/quiz", quizDto);
            if (!res.IsSuccessStatusCode)
                return BadRequest("Quiz oluşturulamadı.");

            var quizId = await res.Content.ReadAsStringAsync();
            return Json(new { success = true, quizId });
        }

    }
}
