using EduPilot_Web.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EduPilot_Web.Controllers
{
    public class AuthController : ApiControllerBase
    {
        public AuthController(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }
        public PublisherController PublisherController;
        public InstitutionController InstitutionController;

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PublisherRegisterSubmit([FromForm] RegisterDTO model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Lütfen tüm alanları eksiksiz doldurun.";
                //return PublisherController.login;
            }

            var client = GetApiClient();
            var response = await client.PostAsJsonAsync("publisher/register", model);

            if (response.IsSuccessStatusCode)
            {
                //TempData["Success"] = "Kayıt başarılı. Giriş sayfasına yönlendiriliyorsunuz.";
                return PublisherController.RedirectToAction("Login");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                TempData["Error"] = "Kayıt başarısız: " + errorContent;
                return PublisherController.RedirectToAction("Register");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PublisherLoginSubmit([FromForm] LoginDTO model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Lütfen geçerli bilgi girin.";
                return PublisherController.RedirectToAction("Login");
            }

            var client = GetApiClient();
            var response = await client.PostAsJsonAsync("auth/login/publisher", model);

            if (response.IsSuccessStatusCode)
            {
                return PublisherController.RedirectToAction("Profile");
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"Giriş başarısız: {content}";
                return PublisherController.RedirectToAction("Login");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InstitutionRegisterSubmit([FromForm] RegisterDTO model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Lütfen tüm alanları eksiksiz doldurun.";
                return InstitutionController.RedirectToAction("Register");
            }

            var client = GetApiClient();
            var response = await client.PostAsJsonAsync("institution/register", model);

            if (response.IsSuccessStatusCode)
            {
                //TempData["Success"] = "Kayıt başarılı. Giriş sayfasına yönlendiriliyorsunuz.";
                return InstitutionController.RedirectToAction("Login");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                TempData["Error"] = "Kayıt başarısız: " + errorContent;
                return InstitutionController.RedirectToAction("Register");
            }
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InstitutionLoginSubmit([FromForm] LoginDTO model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Lütfen geçerli bilgi girin.";
                return InstitutionController.RedirectToAction("Login");
            }

            var client = GetApiClient();
            var response = await client.PostAsJsonAsync("auth/login/institution", model);

            if (response.IsSuccessStatusCode)
            {
                return InstitutionController.RedirectToAction("Profile");
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"Giriş başarısız: {content}";
                return InstitutionController.RedirectToAction("Login");
            }
        }

    }
}
