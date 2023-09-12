using Grpc.Net.Client;
using LitJWT;
using LitJWT.Algorithms;
using MagicOnion.Serialization.MemoryPack;
using MagicOnion.Server;
using MagicT.Server.Database;
using MagicT.Server.Jwt;
using MessagePipe;
using Microsoft.EntityFrameworkCore;
using MagicT.Server.Exceptions;
using MagicT.Server.HostedServices;
using MagicT.Server.Initializers;
using MagicT.Server.ZoneTree;
using MagicT.Server.ZoneTree.Zones;
using MagicT.Shared.Models.ServiceModels;
using MagicT.Server.ZoneTree.Models;
using MagicT.Shared.Models;
#if (GRPC_SSL)
using MagicT.Server.Helpers;
using Microsoft.AspNetCore.Server.Kestrel.Core;
#endif

var builder = WebApplication.CreateBuilder(args);

#if (GRPC_SSL)
//*** Important Note : Make sure server.crt and server.key copyToOutputDirectory property is set to Always copy
var crtPath = Path.Combine(Environment.CurrentDirectory, builder.Configuration.GetSection("Certificate:CrtPath").Value);
var keyPath = Path.Combine(Environment.CurrentDirectory, builder.Configuration.GetSection("Certificate:KeyPath").Value);

var certificate = CertificateHelper.GetCertificate(crtPath,keyPath);


//Verify certificate
var verf = certificate.Verify();

builder.WebHost.ConfigureKestrel((context, opt) =>
{
    opt.ListenLocalhost(7197, o =>
    {
        o.Protocols = HttpProtocols.Http2;
        o.UseHttps(certificate);
    });
});
#endif

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddMagicOnion(x =>
{
#if  DEBUG
    x.IsReturnExceptionStackTraceInErrorDetail = true;
#endif
    //Remove this line to use magic onion with message pack
    x.MessageSerializer = MemoryPackMagicOnionSerializerProvider.Instance;
});

builder.Services.AddSingleton<DbExceptionHandler>();

builder.Services.AddDbContext<MagicTContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString(nameof(MagicTContext))!));

builder.Services.AddHostedService<QueuedHostedService>();

builder.Services.AddSingleton<IBackGroundTaskQueue>(x =>
{
    var result = int.TryParse(builder.Configuration["QueueCapacity"], out int queueCapacity);

    if (!result) queueCapacity = 100;

    return new BackGroundTaskQueue(queueCapacity, builder.Services);
});

var zonedbPath = builder.Configuration.GetSection("ZoneDbPath").Value;

builder.Services.AddSingleton(x => new UsersZoneDb(zonedbPath + $"/{nameof(UsersZoneDb)}"));
builder.Services.AddSingleton(x=> new UsedTokensZoneDb(zonedbPath+$"/{nameof(UsedTokensZoneDb)}"));
//builder.Services.AddSingleton(x => new PermissionsZoneDb(zonedbPath + $"/{nameof(PermissionsZoneDb)}"));

builder.Services.AddSingleton<ZoneDbManager>();

builder.Services.AddSingleton<Lazy<List<PERMISSIONS>>>();
builder.Services.AddMessagePipe();

builder.Services.AddSingleton<GlobalData>();

builder.Services.AddSingleton(_ =>
{
    var key = HS256Algorithm.GenerateRandomRecommendedKey();

    var encoder = new JwtEncoder(new HS256Algorithm(key));
    var decoder = new JwtDecoder(encoder.SignAlgorithm);

    return new MagicTTokenService
    {
        Encoder = encoder,
        Decoder = decoder
    };
});

// builder.Services.AddSingleton(provider => new MemoryDatabaseManager(provider));
builder.Services.AddScoped<DataInitializer>();

var app = builder.Build();

using var scope = app.Services.CreateAsyncScope();

// scope.ServiceProvider.GetService<MemoryDatabaseManager>().CreateNewDatabase();

scope.ServiceProvider.GetRequiredService<DataInitializer>().Initialize();
 
app.UseRouting();

app.MapMagicOnionHttpGateway("_", app.Services.GetService<MagicOnionServiceDefinition>().MethodHandlers,
    GrpcChannel.ForAddress("http://localhost:5029")); // Use HTTP instead of HTTPS
app.MapMagicOnionSwagger("swagger", app.Services.GetService<MagicOnionServiceDefinition>().MethodHandlers, "/_/");

app.MapMagicOnionService();

app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();