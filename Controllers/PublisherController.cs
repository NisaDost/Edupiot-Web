using EduPilot.Web.DTOs;
using EduPilot.Web.Models;
using Microsoft.AspNetCore.Mvc;
using EduPilot.Web.Filters;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using System.Text;

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
                    content.Add(new StringContent(question.IsActive.ToString().ToLower()), $"Questions[{i}].isActive");

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
            catch (Exception)
            {
                return StatusCode(400, "Quiz oluşturma sırasında hata oluştu.");
            }
        }


        public async Task<IActionResult> ListQuizzes()
        {
            try
            {
                var publisherId = GetLoggedinPublisherId();
                var client = GetApiClient();
                var response = await client.GetAsync($"publisher/{publisherId}/quizlist");
                var quizzes = new List<PublisherQuizzesDTO>();
                if (!response.IsSuccessStatusCode)
                {
                    return View("ListQuizzes", quizzes);
                }

                quizzes = await response.Content.ReadFromJsonAsync<List<PublisherQuizzesDTO>>();
                return View("ListQuizzes", quizzes);
            }
            catch
            {
                return Json(new { success = false, quizzes = new List<QuizDTO>() });
            }

        }

        public async Task<IActionResult> SetQuizState(Guid id, bool isActive)
        {
            try
            {
                var client = GetApiClient();
                var response = await client.PutAsJsonAsync($"publisher/quizstate/{id}", isActive);

                if (!response.IsSuccessStatusCode)
                    Json(new { success = false, message = "Quiz durumu güncellenemedi." });

                return Json(new { success = true, message = "Quiz durumu güncellendi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Quiz durumu güncellenemedi.", error = ex.Message });
            }
        }

        public async Task<IActionResult> QuizDetail(Guid quizId)
        {
            var questions = new QuizDetailDTO();
            try
            {
                var client = GetApiClient();
                var publisherId = GetLoggedinPublisherId();
                var response = await client.GetAsync($"publisher/{publisherId}/quiz/{quizId}");

                if (!response.IsSuccessStatusCode)
                {
                    return View("QuizDetail", questions);
                }

                questions = await response.Content.ReadFromJsonAsync<QuizDetailDTO>();
                return View("QuizDetail", questions);
            }
            catch
            {
                return View("QuizDetail", questions);
            }
        }

        public async Task<IActionResult> AddQuestionToQuiz()
        {
            try
            {
                // Extract quiz ID from form
                var quizId = Request.Form["QuizId"].ToString();

                // Extract question content and image from form
                var questionContent = Request.Form["QuestionContent"].ToString();
                var questionImage = Request.Form["QuestionImage"].ToString();
                var isActive = true; // Default to active

                // Process choices
                var correctChoiceContent = Request.Form["CorrectChoiceContent"].ToString();

                // Count wrong answers based on form structure from the view
                var choiceCount = int.Parse(Request.Form["choiceCount"].ToString());
                int wrongAnswerCount = choiceCount - 1; // Total choices minus the correct one

                // Check if we have a file to upload
                IFormFile file = null;
                if (Request.Form.Files.Count > 0)
                {
                    file = Request.Form.Files[0];
                }

                // Prepare the MultipartFormDataContent
                var client = GetApiClient();
                using var content = new MultipartFormDataContent();

                // Add basic question data
                content.Add(new StringContent(questionContent), "QuestionContent");
                content.Add(new StringContent(isActive.ToString().ToLower()), "IsActive");

                // Add question image URL if it exists
                if (!string.IsNullOrEmpty(questionImage))
                {
                    content.Add(new StringContent(questionImage), "QuestionImage");
                }

                // Add file if it exists
                if (file != null && file.Length > 0)
                {
                    var fileContent = new StreamContent(file.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                    content.Add(fileContent, "File", file.FileName);
                }

                // Add the correct answer as the first choice
                content.Add(new StringContent(correctChoiceContent), "Choices[0].ChoiceContent");
                content.Add(new StringContent("true"), "Choices[0].IsCorrect");

                // Add wrong answers from the form
                for (int i = 0; i < wrongAnswerCount; i++)
                {
                    var wrongAnswerContent = Request.Form[$"WrongChoiceContents[{i}]"].ToString();
                    if (!string.IsNullOrEmpty(wrongAnswerContent))
                    {
                        content.Add(new StringContent(wrongAnswerContent), $"Choices[{i + 1}].ChoiceContent");
                        content.Add(new StringContent("false"), $"Choices[{i + 1}].IsCorrect");
                    }
                }

                // Send request to API - using POST to create a new question
                var response = await client.PostAsync($"publisher/question/quiz/{quizId}", content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Question added successfully" });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = $"Error: {errorContent}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error adding question: {ex.Message}" });
            }
        }

        public async Task<IActionResult> UpdateQuestion(QuestionDTO questionDTO)
        {
            try
            {
                // Initialize new QuestionDTO to send to API
                var apiQuestionDTO = new QuestionDTO
                {
                    QuestionId = questionDTO.QuestionId,
                    QuestionContent = questionDTO.QuestionContent,
                    QuestionImage = Request.Form["QuestionImage"].ToString(),
                    IsActive = true // Default to active
                };

                // Initialize choices collection
                apiQuestionDTO.Choices = new List<ChoiceDTO>();

                // Extract correct choice data from form
                if (!string.IsNullOrEmpty(Request.Form["CorrectChoiceId"]) && !string.IsNullOrEmpty(Request.Form["CorrectChoiceContent"]))
                {
                    var correctChoiceId = Guid.Parse(Request.Form["CorrectChoiceId"]);
                    var correctChoiceContent = Request.Form["CorrectChoiceContent"].ToString();

                    apiQuestionDTO.Choices.Add(new ChoiceDTO
                    {
                        ChoiceId = correctChoiceId,
                        ChoiceContent = correctChoiceContent,
                        IsCorrect = true
                    });
                }

                // Extract wrong choice data from form
                int wrongChoiceIndex = 0;
                while (Request.Form.ContainsKey($"WrongChoiceIds[{wrongChoiceIndex}]") &&
                       Request.Form.ContainsKey($"WrongChoiceContents[{wrongChoiceIndex}]"))
                {
                    var wrongChoiceId = Guid.Parse(Request.Form[$"WrongChoiceIds[{wrongChoiceIndex}]"]);
                    var wrongChoiceContent = Request.Form[$"WrongChoiceContents[{wrongChoiceIndex}]"].ToString();

                    apiQuestionDTO.Choices.Add(new ChoiceDTO
                    {
                        ChoiceId = wrongChoiceId,
                        ChoiceContent = wrongChoiceContent,
                        IsCorrect = false
                    });

                    wrongChoiceIndex++;
                }

                // Check if we have a file to upload
                IFormFile file = null;
                if (Request.Form.Files.Count > 0)
                {
                    file = Request.Form.Files[0];
                }

                var client = GetApiClient();
                using var content = new MultipartFormDataContent();

                // Add basic question data
                content.Add(new StringContent(apiQuestionDTO.QuestionId.ToString()), "QuestionId");
                content.Add(new StringContent(apiQuestionDTO.QuestionContent), "QuestionContent");
                content.Add(new StringContent(apiQuestionDTO.IsActive.ToString().ToLower()), "IsActive");

                // Add question image URL if it exists
                if (!string.IsNullOrEmpty(apiQuestionDTO.QuestionImage))
                {
                    content.Add(new StringContent(apiQuestionDTO.QuestionImage), "QuestionImage");
                }

                // Add file if it exists
                if (file != null && file.Length > 0)
                {
                    var fileContent = new StreamContent(file.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                    content.Add(fileContent, "File", file.FileName);
                }

                // Add choices to the request content
                for (int i = 0; i < apiQuestionDTO.Choices.Count; i++)
                {
                    var choice = apiQuestionDTO.Choices[i];
                    content.Add(new StringContent(choice.ChoiceId.ToString()), $"Choices[{i}].ChoiceId");
                    content.Add(new StringContent(choice.ChoiceContent), $"Choices[{i}].ChoiceContent");
                    content.Add(new StringContent(choice.IsCorrect.ToString().ToLower()), $"Choices[{i}].IsCorrect");
                }

                var response = await client.PutAsync($"publisher/question/{apiQuestionDTO.QuestionId}", content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Question updated successfully" });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = $"Error: {errorContent}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error updating question: {ex.Message}" });
            }
        }
    
    }
}
