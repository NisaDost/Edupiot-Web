using EduPilot_Web.Filters;
using EduPilot_Web.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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
                var publisherId = await response.Content.ReadAsStringAsync();
                try
                {
                    HttpContext.Session.SetString("PublisherId", publisherId.ToString());
                    HttpContext.Session.SetString("UserType", "Publisher");
                    return RedirectToAction("Profile", "Publisher");
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Hata: " + ex.Message;
                    return RedirectToAction("Login", "Publisher");
                }
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
                var institutionId = await response.Content.ReadAsStringAsync();
                try
                {
                    HttpContext.Session.SetString("InstitutionId", institutionId.ToString());
                    HttpContext.Session.SetString("UserType", "Institution");
                    return RedirectToAction("Profile", "Institution");
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Hata: " + ex.Message;
                    return RedirectToAction("Login", "Institution");
                }
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"Giriş başarısız: {content}";
                return RedirectToAction("Login", "Institution");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

    }
}
