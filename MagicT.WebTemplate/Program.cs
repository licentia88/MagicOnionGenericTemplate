using Generator.Components.Extensions;
using MagicT.Client.Extensions;
using MagicT.Client.Initializers;
using MagicT.Client.Services;
using MudBlazor.Services;
using MagicT.Shared.Extensions;
using MagicT.Shared.Hubs;
using MagicT.Shared.Services;
using MagicT.Web.Shared.MiddleWares;
using MagicT.Web.Shared.Options;
using MessagePipe;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServerSideBlazor().AddCircuitOptions(options =>
{
    options.DetailedErrors = true;
    options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromSeconds(0);
    options.DisconnectedCircuitMaxRetained = 0;
});

builder.Services.AddRazorPages();
builder.Services.AddMudServices();
builder.Services.AutoRegisterFromMagicTShared();
builder.Services.AutoRegisterFromMagicTClient();
builder.Services.AutoRegisterFromMagicTWebTemplate();
builder.Services.AutoRegisterFromMagicTWebShared();
builder.Services.AutoRegisterFromMagicTRedis();
builder.Services.RegisterGeneratorComponents();
builder.Services.RegisterClientServices(builder.Configuration);
builder.Services.RegisterShared(builder.Configuration);
builder.Services.Configure<MaintenanceModeOptions>(builder.Configuration.GetSection("MaintenanceMode"));

builder.Services.AddMessagePipe();

builder.Services.AddHttpContextAccessor();
// Add services to the container.
builder.Services.AddControllersWithViews();
  

await Task.Delay(5000);
var app = builder.Build();

await using var scope =   app.Services.CreateAsyncScope();
if (scope.ServiceProvider.GetService<IKeyExchangeService>() is KeyExchangeService keyExchangeService)
{
    await keyExchangeService.GlobalKeyExchangeAsync();
}

var testHub = scope.ServiceProvider.GetService<ITestHub>();
await testHub.ConnectAsync();

var dbInitializer = scope.ServiceProvider.GetService<DataInitializer>();

await dbInitializer.Initialize();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseMiddleware<MaintenanceMiddleware>();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapBlazorHub();

app.MapFallbackToPage("/_Host");

app.Run();

