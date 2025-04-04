﻿using Generator.Components.Extensions;
using MagicT.Client.Extensions;
using MagicT.Client.Initializers;
using MagicT.Client.Services;
using MudBlazor.Services;
using MagicT.Shared.Extensions;
using MagicT.Shared.Services;
using MagicT.Shared.Hubs;
using MagicT.Web.Shared.MiddleWares;
using MagicT.Web.Shared.Options;


var builder = WebApplication.CreateBuilder(args);

 // Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor().AddCircuitOptions(options =>
{
    options.DetailedErrors = true;
    options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromSeconds(0);
    options.DisconnectedCircuitMaxRetained = 0;
});

builder.Services.AddMudServices();
 

builder.Services.AutoRegisterFromMagicTClient();
builder.Services.AutoRegisterFromMagicTWeb();
builder.Services.AutoRegisterFromMagicTWebShared();
builder.Services.AutoRegisterFromMagicTRedis();

builder.Services.RegisterGeneratorComponents();
builder.Services.RegisterClientServices(builder.Configuration);
  
builder.Services.RegisterShared(builder.Configuration); 

 
builder.Services.AddHttpContextAccessor();

builder.Services.Configure<MaintenanceModeOptions>(builder.Configuration.GetSection("MaintenanceMode"));


if(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
{
    // Give time for the server to start
     await Task.Delay(3000);
}
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