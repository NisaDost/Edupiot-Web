using EduPilot.Web.DTOs;
using EduPilot.Web.Filters;
using EduPilot.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace EduPilot.Web.Controllers
{
    [AuthorizeUser(Role = "Institution")]
    public class InstitutionController : ApiControllerBase
    {
        public InstitutionController(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }

        private readonly string savePath = @".\FRAS\faces";

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

        public string GetLoggedinInstitutionId()
        {
            var institutionId = HttpContext.Session.GetString("InstitutionId")?.Trim('"');
            return institutionId ?? string.Empty;
        }

        private async Task<InstitutionDTO?> GetInstitutionAsync(string institutionId)
        {
            try
            {
                var client = GetApiClient();
                var response = await client.GetAsync($"institution/{institutionId}");
                if (!response.IsSuccessStatusCode) return null;
                return await response.Content.ReadFromJsonAsync<InstitutionDTO>();
            }
            catch
            {
                return null;
            }
        }

        private async Task<List<StudentDTO>?> GetStudentListAsync(string institutionId)
        {
            try
            {
                var client = GetApiClient();
                var response = await client.GetAsync($"institution/{institutionId}/students");
                if (!response.IsSuccessStatusCode) return new List<StudentDTO>();
                return await response.Content.ReadFromJsonAsync<List<StudentDTO>>();
            }
            catch
            {
                return new List<StudentDTO>();
            }
        }

        private async Task<List<SupervisorDTO>?> GetSupervisorListAsync(string institutionId)
        {
            try
            {
                var client = GetApiClient();
                var response = await client.GetAsync($"institution/{institutionId}/supervisor");
                if (!response.IsSuccessStatusCode) return new List<SupervisorDTO>();
                return await response.Content.ReadFromJsonAsync<List<SupervisorDTO>>();
            }
            catch
            {
                return new List<SupervisorDTO>();
            }
        }

        public async Task<IActionResult> Profile()
        {
            try
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

                return View("Profile", model);
            }
            catch
            {
                return StatusCode(500, "Beklenmeyen bir hata oluştu.");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromForm] InstitutionViewModel updated)
        {
            try
            {
                var institutionId = GetLoggedinInstitutionId();
                var current = await GetInstitutionAsync(institutionId);

                if (current == null)
                    return BadRequest("Geçerli kullanıcı bulunamadı.");

                var client = GetApiClient();

                // Create multipart form data content
                using var formContent = new MultipartFormDataContent();

                // Add text fields - match backend API expectations
                if (!string.IsNullOrEmpty(updated.Name))
                    formContent.Add(new StringContent(updated.Name), "Name");

                formContent.Add(new StringContent(current.Email), "Email");

                if (!string.IsNullOrEmpty(updated.Address))
                    formContent.Add(new StringContent(updated.Address), "Address");

                if (!string.IsNullOrEmpty(updated.Website))
                    formContent.Add(new StringContent(updated.Website), "Website");

                if (!string.IsNullOrEmpty(updated.Logo))
                    formContent.Add(new StringContent(updated.Logo), "Logo");

                // Backend expects "Password" field for current password verification
                if (!string.IsNullOrEmpty(updated.CurrentPassword))
                    formContent.Add(new StringContent(updated.CurrentPassword), "Password");

                if (!string.IsNullOrEmpty(updated.NewPassword))
                    formContent.Add(new StringContent(updated.NewPassword), "NewPassword");

                // Backend expects "File" field for file upload
                if (updated.LogoFile != null && updated.LogoFile.Length > 0)
                {
                    var fileContent = new StreamContent(updated.LogoFile.OpenReadStream());
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(updated.LogoFile.ContentType);
                    formContent.Add(fileContent, "File", updated.LogoFile.FileName);
                }

                var response = await client.PutAsync($"institution/{institutionId}", formContent);

                if (response.IsSuccessStatusCode)
                {
                    return Ok();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return BadRequest($"Profil güncellenemedi: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Güncelleme sırasında bir hata oluştu: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> StudentList()
        {
            try
            {
                var institutionId = GetLoggedinInstitutionId();
                var studentList = await GetStudentListAsync(institutionId);
                if (studentList == null) return NotFound();

                return View("StudentList", studentList);
            }
            catch
            {
                return StatusCode(500, "Liste alınırken hata oluştu.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> SupervisorList()
        {
            try
            {
                var institutionId = GetLoggedinInstitutionId();
                var supervisorList = await GetSupervisorListAsync(institutionId);
                if (supervisorList == null) return NotFound();

                return View("TeacherList", supervisorList);
            }
            catch
            {
                return StatusCode(500, "Liste alınırken hata oluştu.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddStudent([FromBody] string email)
        {
            try
            {
                var institutionId = GetLoggedinInstitutionId();
                var client = GetApiClient();

                var response = await client.PostAsync(
                    $"institution/{institutionId}/student/{email}",
                    null
                );

                return response.IsSuccessStatusCode ? Ok() : BadRequest("Öğrenci eklenemedi.");
            }
            catch
            {
                return StatusCode(500, "İşlem sırasında bir hata oluştu.");
            }
        }

        public async Task<IActionResult> AddSupervisor([FromBody] string email)
        {
            try
            {
                var institutionId = GetLoggedinInstitutionId();
                var client = GetApiClient();

                var response = await client.PostAsync(
                    $"institution/{institutionId}/supervisor/{email}",
                    null
                );

                return response.IsSuccessStatusCode ? Ok() : BadRequest("Öğretmen eklenemedi.");
            }
            catch
            {
                return StatusCode(500, "İşlem sırasında bir hata oluştu.");
            }
        }


        public IActionResult TakePhoto()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveCapturedPhoto([FromForm] UpdateMugshotDTO mugshotDto,
    [FromQuery] string studentId,
    [FromQuery] string studentName,
    [FromQuery] string studentLastName)
        {
            try
            {
                var institutionId = GetLoggedinInstitutionId();
                if (mugshotDto.Mugshot == null)
                    return BadRequest(new { message = "Eksik veri." });

                // Create filename from student name and last name
                var fileName = $"{studentName}.png";

                // Save the file
                var photoPath = Path.Combine(savePath, fileName);
                using (var stream = new FileStream(photoPath, FileMode.Create))
                {
                    await mugshotDto.Mugshot.CopyToAsync(stream);
                }

                // Keep your existing Python API call unchanged
                var pythonClient = GetPythonApiClient();
                var encodeResponse = await pythonClient.GetAsync("encode_faces");
                if (!encodeResponse.IsSuccessStatusCode)
                {
                    var error = await encodeResponse.Content.ReadAsStringAsync();
                    return StatusCode(500, new { message = "Face encode hatası", error });
                }

                // Prepare multipart form data for the API call
                var apiFormData = new MultipartFormDataContent();

                // Add InstitutionId - make sure it's not null
                if (!string.IsNullOrEmpty(institutionId))
                {
                    apiFormData.Add(new StringContent(institutionId), "InstitutionId");
                }

                // Read the saved file and add to form data
                var fileBytes = await System.IO.File.ReadAllBytesAsync(photoPath);
                var fileContent = new ByteArrayContent(fileBytes);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
                apiFormData.Add(fileContent, "Mugshot", fileName);

                // Call the API with multipart/form-data
                var appClient = GetApiClient();
                var updateResponse = await appClient.PutAsync($"students/{studentId}/mugshot", apiFormData);

                if (!updateResponse.IsSuccessStatusCode)
                {
                    var responseContent = await updateResponse.Content.ReadAsStringAsync();
                    return StatusCode(500, new { message = "Profil resmi güncellenemedi.", error = responseContent });
                }

                return Ok(new { message = $"Fotoğraf kaydedildi: {fileName}", filePath = photoPath });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Hata oluştu: {ex.Message}" });
            }
        }

        public IActionResult TakeAttendance()
        {
            return View();
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

        [HttpPost]
        public async Task<IActionResult> RunAttendanceApp()
        {
            try
            {
                // JSON gövdesini oku
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();

                // JSON'u sözlüğe çevir
                var data = JsonSerializer.Deserialize<Dictionary<string, string>>(body);
                var lessonId = data?["lessonId"];
                var lessonName = data?["lessonName"];
                var institutionId = GetLoggedinInstitutionId();

                if (string.IsNullOrEmpty(lessonId))
                {
                    return BadRequest(new { success = false, message = "Ders ID'si belirtilmedi." });
                }

                var client = GetPythonApiClient();

                var requestBody = new
                {
                    lesson_id = lessonId,
                    lesson_name = lessonName,
                    institution_id = institutionId,
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(requestBody),
                    Encoding.UTF8, "application/json");

                var response = await client.PostAsync("take_attendance", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, new
                    {
                        success = false,
                        message = "Yoklama uygulaması başlatılamadı.",
                        details = responseContent
                    });
                }

                return Ok(new { success = true, message = "Yoklama uygulaması başlatıldı." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Hata oluştu", details = ex.Message });
            }
        }

        private async Task<List<AttendanceDTO>?> GetAttendanceListAsync(string institutionId)
        {
            try
            {
                var client = GetApiClient();
                var response = await client.GetAsync($"institution/{institutionId}/attendance");
                if (!response.IsSuccessStatusCode) return new List<AttendanceDTO>();
                return await response.Content.ReadFromJsonAsync<List<AttendanceDTO>>();
            }
            catch
            {
                return new List<AttendanceDTO>();
            }
        }

        [HttpGet]
        public async Task<IActionResult> AttendanceAndEmotionList()
        {

            try
            {
                var institutionId = GetLoggedinInstitutionId();
                var attendanceList = await GetAttendanceListAsync(institutionId);
                if (attendanceList == null) return NotFound();

                return View("AttendanceAndEmotionList", attendanceList);
            }
            catch
            {
                return StatusCode(500, "Liste alınırken hata oluştu.");
            }
        }

        public IActionResult CurrentPlan()
        {
            return View("CurrentPlan");
        }
    }
}
