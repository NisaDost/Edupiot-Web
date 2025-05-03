using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EduPilot_Web.Controllers
{
    public class ApiControllerBase : Controller
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public ApiControllerBase(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        //TODO

        //public async Task OnGet()
        //{
        //    var httpClient = _httpClientFactory.CreateClient("GitHub");
        //    var httpResponseMessage = await httpClient.GetAsync(
        //        "repos/dotnet/AspNetCore.Docs/branches");

        //    if (httpResponseMessage.IsSuccessStatusCode)
        //    {
        //        using var contentStream =
        //            await httpResponseMessage.Content.ReadAsStreamAsync();

        //        GitHubBranches = await JsonSerializer.DeserializeAsync
        //            <IEnumerable<GitHubBranch>>(contentStream);
        //    }
        //}

        

    }
}
