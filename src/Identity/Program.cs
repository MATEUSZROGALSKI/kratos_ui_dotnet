using Identity;
using Identity.Middleware;

using Ory.Kratos.Client.Api;
using Ory.Kratos.Client.Client;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(opt => opt.ListenAnyIP(DockerValues.ListeningPort));

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IFrontendApiAsync>(provider =>
    new FrontendApi(new Configuration { BasePath = DockerValues.AdminUrl }));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseMiddleware<KratosMiddleware>();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();