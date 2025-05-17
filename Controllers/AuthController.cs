using EduPilot.Web.Filters;
using EduPilot.Web.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EduPilot.Web.Controllers
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

            try
            {
                var client = GetApiClient();
                var response = await client.PostAsJsonAsync("publisher/register", model);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("Login", "Publisher");

                var errorContent = await response.Content.ReadAsStringAsync();
                TempData["Error"] = "Kayıt başarısız: " + errorContent;
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Beklenmeyen bir hata oluştu: " + ex.Message;
            }

            return RedirectToAction("Register", "Publisher");
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

            try
            {
                var client = GetApiClient();
                var response = await client.PostAsJsonAsync("auth/login/publisher", model);

                if (response.IsSuccessStatusCode)
                {
                    var publisherId = await response.Content.ReadAsStringAsync();
                    HttpContext.Session.SetString("PublisherId", publisherId);
                    HttpContext.Session.SetString("UserType", "Publisher");
                    return RedirectToAction("Profile", "Publisher");
                }

                var content = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"Giriş başarısız: {content}";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Hata: " + ex.Message;
            }

            return RedirectToAction("Login", "Publisher");
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

            try
            {
                var client = GetApiClient();
                var response = await client.PostAsJsonAsync("institution/register", model);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("Login", "Institution");

                var errorContent = await response.Content.ReadAsStringAsync();
                TempData["Error"] = "Kayıt başarısız: " + errorContent;
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Beklenmeyen bir hata oluştu: " + ex.Message;
            }

            return RedirectToAction("Register", "Institution");
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

            try
            {
                var client = GetApiClient();
                var response = await client.PostAsJsonAsync("auth/login/institution", model);

                if (response.IsSuccessStatusCode)
                {
                    var institutionId = await response.Content.ReadAsStringAsync();
                    HttpContext.Session.SetString("InstitutionId", institutionId);
                    HttpContext.Session.SetString("UserType", "Institution");
                    return RedirectToAction("Profile", "Institution");
                }

                var content = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"Giriş başarısız: {content}";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Hata: " + ex.Message;
            }

            return RedirectToAction("Login", "Institution");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
