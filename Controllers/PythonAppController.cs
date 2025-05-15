using EduPilot_Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace EduPilot_Web.Controllers
{
    public class PythonAppController : ApiControllerBase
    {
        public PythonAppController(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }

        private readonly string savePath = @".\FRAS\faces";

        public IActionResult TakePhoto()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveCapturedPhotoAsync([FromBody] ImageUploadModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Image) || string.IsNullOrEmpty(model.FileName) || string.IsNullOrEmpty(model.StudentId))
                    return BadRequest(new { message = "Eksik veri: image, filename veya studentId yok." });

                var fileName = model.FileName;
                var studentId = model.StudentId;
                var imageBytes = Convert.FromBase64String(model.Image);

                var photoPath = Path.Combine(savePath, fileName);
                await System.IO.File.WriteAllBytesAsync(photoPath, imageBytes);

                var pythonClient = GetPythonApiClient();
                var encodeResponse = await pythonClient.GetAsync("encode_faces");

                if (!encodeResponse.IsSuccessStatusCode)
                {
                    var error = await encodeResponse.Content.ReadAsStringAsync();
                    return StatusCode(500, new { message = "Face encode hatası", error });
                }

                var updatedMugshot = new
                {
                    Mugshot = model.FileName
                };

                var appClient = GetApiClient();
                var updateResponse = await appClient.PutAsJsonAsync($"students/{studentId}/mugshot", updatedMugshot);

                if (!updateResponse.IsSuccessStatusCode)
                {
                    return StatusCode(500, new { message = "Profil resmi güncellenemedi." });
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

        public async Task<IActionResult> RunAttendanceAppAsync()
        {
            try
            {
                var client = GetPythonApiClient();
                var response = await client.GetAsync("take_attendance");
                var result = await response.Content.ReadAsStringAsync();
                return View("TakeAttendance");
            }
            catch
            {
                return StatusCode(500, "Yoklama alınırken bir hata oluştu.");
            }
        }

        public async Task<IActionResult> ListAttendanceAndEmotion()
        {
            var client = GetPythonApiClient();
            var response = await client.GetAsync("get_attendance_report");
            return View("ListAttendanceAndEmotion");
        }

        //[HttpPost]
        //public async Task<IActionResult> TakeAttendanceAsync()
        //{
        //    var client = GetPythonApiClient();
        //    var response = await client.GetAsync("/take_attendance"); // python API Route
        //    var result = await response.Content.ReadAsStringAsync();
        //    ViewBag.Message = result;
        //    return View("Index");
        //}

        // buradan aşağısı güncel değil

        //public async Task<IActionResult> AttendanceListAsync()
        //{
        //    var client = GetPythonApiClient();
        //    var response = await client.GetAsync("get_attendance_report"); // python API Route
        //    var jsonString = await response.Content.ReadAsStringAsync();

        //    // JSON'u parse et
        //    var jsonObject = JObject.Parse(jsonString);
        //    var reportData = jsonObject["report"]?.ToString();

        //    // CSV formatındaki veriyi tabloya çevirmek
        //    var attendanceList = new List<AttendanceModel>();

        //    if (!string.IsNullOrEmpty(reportData))
        //    {
        //        var lines = reportData.Split("\n");
        //        foreach (var line in lines.Skip(1)) // İlk satır başlık olduğu için atla
        //        {
        //            var columns = line.Split(",");
        //            if (columns.Length == 4)
        //            {
        //                var name = columns[0].Split("_");
        //                attendanceList.Add(new AttendanceModel
        //                {
        //                    Name = name[0] + " " + name[1],
        //                    Date = columns[1],
        //                    Time = columns[2],
        //                    Emotion = columns[3]
        //                });
        //            }
        //        }
        //    }
        //    return View(attendanceList);
        //}

        //private string ConvertImageToBase64(string imagePath)
        //{
        //    try
        //    {
        //        byte[] imageBytes = System.IO.File.ReadAllBytes(imagePath);
        //        return Convert.ToBase64String(imageBytes);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log error and return an empty string if conversion fails
        //        Console.WriteLine($"Error converting image: {ex.Message}");
        //        return string.Empty;
        //    }
        //}


        //public IActionResult Capture()
        //{
        //    return View();
        //}

    }
}
