using Generator.Components.Extensions;
using MagicT.Client.Extensions;
using MudBlazor.Services;
using MagicT.Shared.Extensions;
using MessagePipe;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServerSideBlazor();
builder.Services.AddRazorPages();
builder.Services.AddMudServices();
builder.Services.AutoRegisterFromMagicTShared();
builder.Services.AutoRegisterFromMagicTClient();
builder.Services.RegisterGeneratorComponents();
builder.Services.RegisterClientServices(builder.Configuration);

builder.Services.RegisterShared(builder.Configuration);


builder.Services.AddHttpContextAccessor();
builder.Services.AddMessagePipe();

builder.Services.AddHttpContextAccessor();
// Add services to the container.
builder.Services.AddControllersWithViews();

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

app.MapBlazorHub();

app.MapFallbackToPage("/_Host");

app.Run();

