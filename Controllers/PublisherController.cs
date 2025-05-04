using EduPilot_Web.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EduPilot_Web.Controllers
{
    public class PublisherController : ApiControllerBase
    {
        public PublisherController(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

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
        public async Task<IActionResult> RegisterSubmit([FromForm] PublisherRegisterDTO model)
        {
            //buraya view yönlendirmesi ekle
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Lütfen tüm alanları doğru doldurunuz." });
            }

            var client = GetApiClient();
            var response = await client.PostAsJsonAsync("publisher/register", model);

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
