using EduPilot.Web.DTOs;
using EduPilot.Web.Models;
using Microsoft.AspNetCore.Mvc;
using EduPilot.Web.Filters;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;

namespace EduPilot.Web.Controllers
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
            try
            {
                var client = GetApiClient();
                var response = client.GetAsync($"publisher/{publisherId}").Result;
                if (!response.IsSuccessStatusCode) return null;

                return response.Content.ReadFromJsonAsync<PublisherDTO>().Result;
            }
            catch
            {
                return null;
            }
        }

        public IActionResult Profile()
        {
            try
            {
                var publisherId = GetLoggedinPublisherId();
                var publisher = GetPublisher(publisherId);

                if (publisher == null)
                    return NotFound();

                var model = new PublisherViewModel
                {
                    Id = publisher.Id,
                    Name = publisher.Name,
                    Email = publisher.Email,
                    Address = publisher.Address,
                    Website = publisher.Website,
                    Logo = publisher.Logo,
                    QuizCount = publisher.QuizCount,
                    QuestionCount = publisher.QuestionCount,
                };

                return View("Profile", model);
            }
            catch
            {
                return StatusCode(500, "Profil yüklenirken bir hata oluştu.");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromForm] PublisherViewModel updated)
        {
            try
            {
                var publisherId = GetLoggedinPublisherId();
                var current = GetPublisher(publisherId);
                if (current == null) return BadRequest("Geçerli kullanıcı bulunamadı.");

                var updatedInfo = new
                {
                    name = updated.Name,
                    email = current.Email,
                    address = updated.Address == null ? String.Empty : updated.Address,
                    website = updated.Website == null ? String.Empty : updated.Website,
                    logo = updated.Logo,
                    currentPassword = updated.CurrentPassword,
                    password = updated.NewPassword
                };

                var client = GetApiClient();
                var response = await client.PutAsJsonAsync($"publisher/{publisherId}", updatedInfo);

                return response.IsSuccessStatusCode ? Ok() : BadRequest("Profil güncellenemedi.");
            }
            catch
            {
                return StatusCode(500, "Profil güncellenirken hata oluştu.");
            }
        }

        public IActionResult CreateQuiz()
        {
            return View("CreateQuiz");
        }

        [HttpGet]
        public async Task<IActionResult> LoadLessons(int grade)
        {
            try
            {
                var client = GetApiClient();
                var response = await client.GetAsync($"lessons/{grade}");

                if (!response.IsSuccessStatusCode)
                    return Json(new { success = false, lessons = new List<LessonsByGradeDTO>() });

                var lessons = await response.Content.ReadFromJsonAsync<List<LessonsByGradeDTO>>();
                return Json(new { success = true, lessons });
            }
            catch
            {
                return Json(new { success = false, lessons = new List<LessonsByGradeDTO>() });
            }
        }

        [HttpGet]
        public async Task<IActionResult> LoadSubjects(Guid lessonId)
        {
            try
            {
                var client = GetApiClient();
                var response = await client.GetAsync($"lessons/{lessonId}/subjects");

                if (!response.IsSuccessStatusCode)
                    return Json(new { success = false, subjects = new List<SubjectDTO>() });

                var subjects = await response.Content.ReadFromJsonAsync<List<SubjectDTO>>();
                return Json(new { success = true, subjects });
            }
            catch
            {
                return Json(new { success = false, subjects = new List<SubjectDTO>() });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuiz([FromForm] QuizDTO quizDto)
        {
            try
            {
                var publisherId = GetLoggedinPublisherId();
                var client = GetApiClient();

                // Create multipart form data content
                using var content = new MultipartFormDataContent();

                // Add basic properties
                content.Add(new StringContent(quizDto.SubjectId.ToString()), "SubjectId");
                content.Add(new StringContent(quizDto.Duration.ToString()), "Duration");
                content.Add(new StringContent(quizDto.Difficulty.ToString()), "Difficulty");
                content.Add(new StringContent(quizDto.IsActive.ToString()), "IsActive");

                // Add questions and their properties
                for (int i = 0; i < quizDto.Questions.Count; i++)
                {
                    var question = quizDto.Questions[i];

                    // Add question properties
                    content.Add(new StringContent(question.QuestionContent), $"Questions[{i}].QuestionContent");
                    content.Add(new StringContent(question.isActive.ToString().ToLower()), $"Questions[{i}].isActive");

                    // Add file if exists
                    if (question.File != null)
                    {
                        var fileStream = question.File.OpenReadStream();
                        var fileContent = new StreamContent(fileStream);
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(question.File.ContentType);
                        content.Add(fileContent, $"Questions[{i}].File", question.File.FileName);
                    }

                    // Add choices
                    for (int j = 0; j < question.Choices.Count; j++)
                    {
                        var choice = question.Choices[j];
                        content.Add(new StringContent(choice.ChoiceContent), $"Questions[{i}].Choices[{j}].ChoiceContent");
                        content.Add(new StringContent(choice.IsCorrect.ToString().ToLower()), $"Questions[{i}].Choices[{j}].IsCorrect");
                    }
                }

                // Send the request
                var res = await client.PostAsync($"publisher/{publisherId}/add/quiz", content);

                if (!res.IsSuccessStatusCode)
                {
                    return BadRequest("Quiz oluşturulamadı.");
                }

                var quizId = await res.Content.ReadAsStringAsync();
                return Json(new { success = true, quizId });
            }
            catch (Exception ex)
            {
                return StatusCode(400, "Quiz oluşturma sırasında hata oluştu.");
            }
        }


        public async Task<IActionResult> GetQuizzes()
        {
            try
            {
                var publisherId = GetLoggedinPublisherId();
                var client = GetApiClient();
                var response = await client.GetAsync($"publisher/{publisherId}/quizzes");

                if (!response.IsSuccessStatusCode)
                    return Json(new { success = false, quizzes = new List<QuizDTO>() });
                var quizzes = await response.Content.ReadFromJsonAsync<List<QuizDTO>>();
                return Json(new { success = true, quizzes });
            }
            catch
            {
                return Json(new { success = false, quizzes = new List<QuizDTO>() });
            }

        }

        public async Task<IActionResult> SetQuizState(Guid quizId)
        {
            try
            {
                var client = GetApiClient();
                var publisherId = GetLoggedinPublisherId();
                var response = await client.PutAsJsonAsync($"publisher/{publisherId}/quizstate/{quizId}", new { quizId });
                return response.IsSuccessStatusCode
                    ? Json(new { success = true, message = "Quiz durumu güncellendi." })
                    : Json(new { success = false, message = "Quiz durumu güncellenemedi." });
            }
            catch
            {
                return Json(new { success = false, message = "Quiz durumu güncellenemedi." });
            }
        }

        public IActionResult EditQuiz(Guid quizId)
        {
            //TODO: QuizId'e göre quiz bilgilerini al ve düzenleme sayfasına git
            return View("EditQuiz");
        }
    }
}
