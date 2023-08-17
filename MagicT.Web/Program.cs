using Generator.Components.Extensions;
using MagicT.Client.Extensions;
using MagicT.Web.MiddleWares;
using MagicT.Web.Models;
using MagicT.Web.Options;
using MagicT.Web.Pages.HelperComponents;
using MessagePipe;
using Microsoft.Extensions.Configuration;
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
builder.Services.AddMessagePipe();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.Configure<MaintenanceModeOptions>(builder.Configuration.GetSection("MaintenanceMode"));


var app = builder.Build();

 
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
 
//app.UseAuthentication();
//app.UseAuthorization();

 

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();