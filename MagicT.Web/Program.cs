using Generator.Components.Extensions;
using MagicT.Client.Extensions;
using MagicT.Client.Services;
using MagicT.Shared.Models;
using MagicT.Web.Initializers;
using MagicT.Web.Managers;
using MagicT.Web.MiddleWares;
using MagicT.Web.Models;
using MagicT.Web.Options;
using MagicT.Web.Pages.HelperComponents;
using MessagePipe;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

 // Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

//builder.Services.AddHttpContextAccessor();

builder.Services.AddMudServices();
builder.Services.RegisterGeneratorComponents();
builder.Services.RegisterClientServices(builder.Configuration);
builder.Services.AddScoped<NotificationsView>();
builder.Services.AddScoped<List<NotificationVM>>();
builder.Services.AddScoped<DataInitializer>();
builder.Services.AddSingleton<Lazy<List<PERMISSIONS>>>();
builder.Services.AddScoped<UserManager>();
builder.Services.AddMessagePipe();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.Configure<MaintenanceModeOptions>(builder.Configuration.GetSection("MaintenanceMode"));

 

///Wait for server to run
await Task.Delay(3000);
var app = builder.Build();

using var scope =   app.Services.CreateAsyncScope();
var keyExchangeService = scope.ServiceProvider.GetService<KeyExchangeService>();
await keyExchangeService.GlobalKeyExchangeAsync();

var dbInitializer = scope.ServiceProvider.GetService<DataInitializer>();
dbInitializer.Initialize();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseMiddleware<MaintenanceMiddleware>();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();



app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();