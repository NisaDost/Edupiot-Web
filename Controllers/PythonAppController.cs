using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace EduPilot_Web.Controllers
{
    public class PythonAppController : ApiControllerBase
    {
        public PythonAppController(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }
        public IActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        public IActionResult TakePhoto()
        {
            return View("Attendance");
        }

        [HttpPost]
        public async Task<IActionResult> TakeAttendance()
        {
            var client = GetPythonApiClient();
            var response = await client.GetAsync($"/take_attendance"); // python API Route
            var result = await response.Content.ReadAsStringAsync();
            ViewBag.Message = result;
            return View("Index");
        }

        // buradan aşağısı güncel değil

        //public async Task<IActionResult> AttendanceList()
        //{
        //    var client = GetPythonApiClient();
        //    var response = await client.GetAsync($"/get_attendance_report"); // python API Route
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
        //public async Task<IActionResult> StudentList()
        //{
        //    var response = await _httpClient.GetAsync($"{pythonApiUrl}/get_students");
        //    if (!response.IsSuccessStatusCode)
        //    {
        //        // Handle error
        //        return View(new List<StudentModel>());
        //    }

        //    var jsonString = await response.Content.ReadAsStringAsync();
        //    var students = JsonConvert.DeserializeObject<List<StudentModel>>(jsonString);

        //    // Convert image paths to Base64
        //    foreach (var student in students)
        //    {
        //        student.PhotoBase64 = ConvertImageToBase64(student.PhotoUrl);
        //    }

        //    return View(students);
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

        //[HttpPost]
        //public async Task<IActionResult> SaveCapturedPhoto([FromBody] ImageUploadModel model)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(model.Image) || string.IsNullOrEmpty(model.FileName))
        //            return BadRequest(new { message = "Invalid image data or filename." });

        //        // Save the image
        //        byte[] imageBytes = Convert.FromBase64String(model.Image);
        //        string filePath = Path.Combine(savePath, model.FileName);
        //        await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

        //        // Call the Flask API to run encode_faces.py
        //        var httpClient = _httpClientFactory.CreateClient();
        //        var response = await httpClient.GetAsync($"{pythonApiUrl}/encode_faces");

        //        if (!response.IsSuccessStatusCode)
        //        {
        //            var errorMsg = await response.Content.ReadAsStringAsync();
        //            return StatusCode(500, new { message = "Error encoding faces", error = errorMsg });
        //        }

        //        return Ok(new { message = $"Photo saved for {model.FileName}!", filePath });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = $"Error saving photo: {ex.Message}" });
        //    }
        //}

        //public IActionResult Capture()
        //{
        //    return View();
        //}

        //public IActionResult Privacy()
        //{
        //    return View();
        //}

    }
}
