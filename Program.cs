using Microsoft.Net.Http.Headers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
var apiUsername = "admin"; /*config["Authorization:Basic:admin"];*/
var apiPassword = "password"; /*config["Authorization:Basic:password"];*/

builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient("EduPilotApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7104/api/");
    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
        "Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(apiUsername + ":" + apiPassword)));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
