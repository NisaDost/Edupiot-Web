using Microsoft.AspNetCore.Mvc;

namespace EduPilot_Web.Controllers
{
    public class ApiControllerBase : Controller
    {
        protected readonly IHttpClientFactory _httpClientFactory;

        public ApiControllerBase(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        protected HttpClient GetApiClient()
        {
            return _httpClientFactory.CreateClient("EduPilotApi");
        }

        protected HttpClient GetPythonApiClient()
        {
            return _httpClientFactory.CreateClient("PythonApi");
        }
    }
}
