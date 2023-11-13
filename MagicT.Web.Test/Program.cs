using Generator.Components.Extensions;
using MagicT.Client.Extensions;
using MagicT.Client.Hubs;
using MagicT.Client.Services;
using MagicT.Shared.Extensions;
using MagicT.Shared.Models.Base;
using MagicT.Web.Test.Initializers;
using MagicT.Web.Test.Managers;
using MagicT.Web.Test.Models;
using MagicT.Web.Test.Options;
using MagicT.Web.Test.Pages.HelperComponents;
using MessagePipe;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();
 
builder.Services.RegisterGeneratorComponents();
builder.Services.RegisterClientServices(builder.Configuration);
builder.Services.AddScoped<DataInitializer>();

builder.Services.AddScoped<NotificationsView>();
builder.Services.AddScoped<List<NotificationVM>>();

builder.Services.AddScoped<Lazy<List<AUTHORIZATIONS_BASE>>>();

builder.Services.RegisterPipes();
builder.Services.AddMessagePipe();


builder.Services.AddScoped<UserManager>();
//builder.Services.AddMessagePipe();
//builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddHttpContextAccessor();



builder.Services.Configure<MaintenanceModeOptions>(builder.Configuration.GetSection("MaintenanceMode"));



///Wait for server to run
await Task.Delay(3000);
var app = builder.Build();

using var scope = app.Services.CreateAsyncScope();
var keyExchangeService = scope.ServiceProvider.GetService<KeyExchangeService>();
await keyExchangeService.GlobalKeyExchangeAsync();

var testHub = scope.ServiceProvider.GetService<TestHub>();
await testHub.ConnectAsync();

var dbInitializer = scope.ServiceProvider.GetService<DataInitializer>();
await dbInitializer.InitializeRolesAsync();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

