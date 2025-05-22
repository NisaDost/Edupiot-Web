using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
var apiUsername = config.GetValue<string>("Authentication:Basic:Username");
var apiPassword = config.GetValue<string>("Authentication:Basic:Password");
var apiUrl = config.GetValue<string>("APIURL");
var pythonApiUrl = config.GetValue<string>("PythonAPIURL");

builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient("EduPilotApi", client =>
{
    client.BaseAddress = new Uri(apiUrl);
    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
        "Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(apiUsername + ":" + apiPassword)));
});

builder.Services.AddHttpClient("PythonApi", client =>
{
    client.BaseAddress = new Uri(pythonApiUrl);
});

builder.Services.AddSession();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
