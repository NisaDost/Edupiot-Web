using EduPilot_Web.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EduPilot_Web.Controllers
{
    public class PublisherController : ApiControllerBase
    {
        public IActionResult Login()
        {
            return View("Auth/Login");
        }

        public IActionResult Register()
        {
            return View("Auth/Register");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterSubmit([FromBody] PublisherRegisterDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Lütfen tüm alanları doğru doldurunuz." });
            }

            using var client = new HttpClient();
            var backendUrl = "https://localhost:7104/api/publisher/register";

            var response = await client.PostAsJsonAsync(backendUrl, model);

            if (response.IsSuccessStatusCode)
            {
                return Ok(new { success = true, message = "Kayıt başarılı, giriş sayfasına yönlendiriliyorsunuz." });
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, new { success = false, message = "Kayıt başarısız: " + errorContent });
            }
        }

        public IActionResult Index()
        {
            return View("Dashboard/Profile");
        }
        public IActionResult Profile()
        {
            return View("Dashboard/Profile");
        }
        
        public IActionResult AddNewQuestion()
        {
            return View("Dashboard/AddNewQuiz");
        }
    }
}
