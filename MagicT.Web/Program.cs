using Generator.Components.Extensions;
using MagicT.Client.Extensions;
using MagicT.Client.Hubs;
using MagicT.Client.Services;
using MagicT.Shared.Models.Base;
using MagicT.Web.Initializers;
using MagicT.Web.Middlewares;
using MagicT.Web.Models;
using MagicT.Web.Options;
using MagicT.Web.Pages.HelperComponents;
using MudBlazor.Services;
using MagicT.Shared.Extensions;
using MessagePipe;
using MagicT.Shared.Managers;
using MagicT.Client.Managers;
using MagicT.Shared.Services;
using MagicT.Shared.Hubs;

var builder = WebApplication.CreateBuilder(args);

 // Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

 //builder.Services.AddHttpContextAccessor();

builder.Services.AddMudServices();
builder.Services.RegisterGeneratorComponents();
builder.Services.RegisterClientServices(builder.Configuration);
builder.Services.AddScoped<DataInitializer>();
builder.Services.AddSingleton<IKeyExchangeManager, KeyExchangeManager>();
builder.Services.AddScoped<IStorageManager, StorageManager>();

builder.Services.AddScoped<NotificationsView>();
 

builder.Services.RegisterPipes();
builder.Services.AddMessagePipe();
 

builder.Services.AddScoped<ILoginManager,LoginManager>();
//builder.Services.AddMessagePipe();
//builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddHttpContextAccessor();

builder.Services.Configure<MaintenanceModeOptions>(builder.Configuration.GetSection("MaintenanceMode"));


await Task.Delay(5000);
var app = builder.Build();

using var scope =   app.Services.CreateAsyncScope();
var keyExchangeService = scope.ServiceProvider.GetService<IKeyExchangeService>() as KeyExchangeService;
await keyExchangeService.GlobalKeyExchangeAsync();

var testHub = scope.ServiceProvider.GetService<ITestHub>();
await testHub.ConnectAsync();

var dbInitializer = scope.ServiceProvider.GetService<DataInitializer>();
await dbInitializer.Initialize();

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