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
    return View("Dashboard/AddQuiz", new QuizViewModel());
}

[HttpPost]
public async Task<IActionResult> CreateQuiz(QuizViewModel model, string action)
{
    var client = GetApiClient();

    if (action == "grade")
    {
        var response = await client.GetAsync($"/api/lessons/{model.SelectedGrade}");
        model.Lessons = response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<List<LessonsByGradeDTO>>()
            : new List<LessonsByGradeDTO>();
    }

    if (action == "lesson")
    {
        var response = await client.GetAsync($"/api/lessons/{model.SelectedLessonId}/subjects");
        model.Subjects = response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<List<SubjectDTO>>()
            : new List<SubjectDTO>();

        // Dersleri yeniden yüklemek için sınıf bilgisi de korunmalı
        var lessonRes = await client.GetAsync($"/api/lessons/{model.SelectedGrade}");
        model.Lessons = lessonRes.IsSuccessStatusCode
            ? await lessonRes.Content.ReadFromJsonAsync<List<LessonsByGradeDTO>>()
            : new List<LessonsByGradeDTO>();
    }

    return View("Dashboard/AddQuiz", model);
}

        [HttpPost]
        public async Task<IActionResult> LoadLessons(int grade)
        {
            var client = GetApiClient();
            var response = await client.GetAsync($"/api/lessons/{grade}");

            if (!response.IsSuccessStatusCode)
                return PartialView("_LessonDropdown", new List<LessonsByGradeDTO>());

            var lessons = await response.Content.ReadFromJsonAsync<List<LessonsByGradeDTO>>();
            return PartialView("_LessonDropdown", lessons);
        }

        [HttpPost]
        public async Task<IActionResult> LoadSubjects(Guid lessonId)
        {
            var client = GetApiClient();
            var response = await client.GetAsync($"/api/lessons/{lessonId}/subjects");

            if (!response.IsSuccessStatusCode)
                return PartialView("_SubjectDropdown", new List<SubjectDTO>());

            var subjects = await response.Content.ReadFromJsonAsync<List<SubjectDTO>>();
            return PartialView("_SubjectDropdown", subjects);
        }

        public IActionResult AddQuestions()
        {
            ViewBag.QuizId = TempData["QuizId"];
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddQuestion(Guid quizId, QuestionDTO question)
        {
            var client = GetApiClient();
            var res = await client.PostAsJsonAsync($"/api/question/quiz/{quizId}", question);
            return res.IsSuccessStatusCode ? Ok() : BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> SaveQuiz(Guid quizId, bool isActive)
        {
            // Opsiyonel: quiz'e son hali kaydet
            return RedirectToAction("QuizList");
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
