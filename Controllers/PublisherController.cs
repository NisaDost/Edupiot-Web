using EduPilot_Web.Models.DTOs;
using EduPilot_Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace EduPilot_Web.Controllers
{
    public class PublisherController : ApiControllerBase
    {
        public PublisherController(IHttpClientFactory httpClientFactory) : base(httpClientFactory){}

        [HttpGet]
        public IActionResult CreateQuiz()
        {
            
            return View("Dashboard/AddQuiz");
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
        public async Task<IActionResult> CreateQuiz(QuizViewModel model)
        {
            var client = GetApiClient();
            var quizDto = new QuizDTO
            {
                SubjectId = model.SelectedSubjectId,
                Difficulty = model.Difficulty,
                Questions = new List<QuestionDTO>()
            };
            var res = await client.PostAsJsonAsync("publisher/addquiz", quizDto);
            if (!res.IsSuccessStatusCode) return BadRequest();

            var quizId = await res.Content.ReadAsStringAsync();
            TempData["QuizId"] = quizId;
            TempData["QuizActive"] = model.IsActive;

            return Json(new { success = true, quizId });
        }

        [HttpPost]
        public async Task<IActionResult> AddQuestion(Guid quizId, QuestionDTO question)
        {
            var client = GetApiClient();
            var res = await client.PostAsJsonAsync($"publisher/question/quiz/{quizId}", question);
            return res.IsSuccessStatusCode ? Json(new { success = true }) : Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> SaveQuiz(Guid quizId, bool isActive)
        {
            // Opsiyonel: quiz'e son hali kaydet
            return Json(new { success = true });
        }


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

        public IActionResult AddQuiz()
        {
            return View("Dashboard/AddQuiz");
        }
    }
}
