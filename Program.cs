using Microsoft.Net.Http.Headers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var apiUsername = builder.Configuration.GetSection("Authorization:Basic:Username");
var apiPassword = builder.Configuration.GetSection("Authorization:Basic:Password");

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient("EduPilotApi", httpClient =>
{
    httpClient.BaseAddress = new Uri("https://localhost:7104/api");

    httpClient.DefaultRequestHeaders.Add(
        HeaderNames.ContentType, "application/json; charset=UTF-8");
    httpClient.DefaultRequestHeaders.Add(
        HeaderNames.Authorization, $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{apiUsername}:{apiPassword}"))}");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
