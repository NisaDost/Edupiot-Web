using System.Diagnostics;
using EduPilot_Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace EduPilot_Web.Controllers
{
    public class HomeController : ApiControllerBase
    {

        private readonly ILogger<HomeController> _logger;
        public HomeController(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }


        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
