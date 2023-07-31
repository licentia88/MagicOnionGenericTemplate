using Generator.Components.Extensions;
using MagicT.Client.Extensions;
using MagicT.Web.Models;
using MagicT.Web.Pages.HelperComponents;
using MessagePipe;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddMudServices();
builder.Services.RegisterGeneratorComponents();
builder.Services.RegisterClientServices();

builder.Services.AddScoped<NotificationsView>();
builder.Services.AddScoped<List<NotificationVM>>();
builder.Services.AddMessagePipe();

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

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

