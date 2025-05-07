using EduPilot_Web.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EduPilot_Web.Controllers
{
    public class AuthController : ApiControllerBase
    {
        public AuthController(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PublisherRegisterSubmit([FromForm] RegisterDTO model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Lütfen tüm alanları eksiksiz doldurun.";
                return RedirectToAction("Register", "Publisher");
            }

            var client = GetApiClient();
            var response = await client.PostAsJsonAsync("publisher/register", model);

            if (response.IsSuccessStatusCode)
            {
                //TempData["Success"] = "Kayıt başarılı. Giriş sayfasına yönlendiriliyorsunuz.";
                return RedirectToAction("Login", "Publisher");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                TempData["Error"] = "Kayıt başarısız: " + errorContent;
                return RedirectToAction("Register", "Publisher");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PublisherLoginSubmit([FromForm] LoginDTO model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Lütfen geçerli bilgi girin.";
                return RedirectToAction("Login", "Publisher");
            }

            var client = GetApiClient();
            var response = await client.PostAsJsonAsync("auth/login/publisher", model);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Profile", "Publisher");
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"Giriş başarısız: {content}";
                return RedirectToAction("Login", "Publisher");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InstitutionRegisterSubmit([FromForm] RegisterDTO model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Lütfen tüm alanları eksiksiz doldurun.";
                return RedirectToAction("Register", "Institution");
            }

            var client = GetApiClient();
            var response = await client.PostAsJsonAsync("institution/register", model);

            if (response.IsSuccessStatusCode)
            {
                //TempData["Success"] = "Kayıt başarılı. Giriş sayfasına yönlendiriliyorsunuz.";
                return RedirectToAction("Login", "Institution");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                TempData["Error"] = "Kayıt başarısız: " + errorContent;
                return RedirectToAction("Register", "Institution");
            }
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InstitutionLoginSubmit([FromForm] LoginDTO model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Lütfen geçerli bilgi girin.";
                return RedirectToAction("Login", "Institution");
            }

            var client = GetApiClient();
            var response = await client.PostAsJsonAsync("auth/login/institution", model);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Profile", "Institution");
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"Giriş başarısız: {content}";
                return RedirectToAction("Login", "Institution");
            }
        }

    }
}
