using MagicT.Client.Extensions;
using MagicT.Client.Initializers;
using MagicT.Client.Services;
using MagicT.Shared.Extensions;
using MagicT.Shared.Hubs;
using MagicT.Shared.Services;
using MagicT.Web.Shared.Options;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MagicT.WebAssembly;
using MudBlazor.Services;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Load the appsettings.json file
using var response = await new HttpClient().GetAsync(new Uri(builder.HostEnvironment.BaseAddress + "appsettings.json"));
await using var stream = await response.Content.ReadAsStreamAsync();
var config = new ConfigurationBuilder()
    .AddJsonStream(stream)
    .Build();

// Register IConfiguration
builder.Services.AddSingleton<IConfiguration>(config);

builder.Services.AddMudServices();
builder.Services.AutoRegisterFromMagicTClient();
builder.Services.AutoRegisterFromMagicTShared();
builder.Services.AutoRegisterFromMagicTWebAssembly();
builder.Services.AutoRegisterFromMagicTWebShared();
builder.Services.RegisterClientServices(builder.Configuration);
builder.Services.RegisterShared(builder.Configuration);
builder.Services.Configure<MaintenanceModeOptions>(builder.Configuration.GetSection("MaintenanceMode"));

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var app = builder.Build();
await using var scope =   app.Services.CreateAsyncScope();
var keyExchangeService = scope.ServiceProvider.GetService<IKeyExchangeService>() as KeyExchangeService;
await keyExchangeService?.GlobalKeyExchangeAsync()!;

var testHub = scope.ServiceProvider.GetService<ITestHub>();
await testHub.ConnectAsync();

var dbInitializer = scope.ServiceProvider.GetService<DataInitializer>();

await dbInitializer.Initialize();
await app.RunAsync();

