using Grpc.Net.Client;
using LitJWT;
using LitJWT.Algorithms;
using MagicOnion.Serialization.MemoryPack;
using MagicOnion.Server;
using MagicT.Server.Database;
using MagicT.Server.Extensions;
using MagicT.Server.Jwt;
using MessagePipe;
using Microsoft.EntityFrameworkCore;
using MagicT.Server.Exceptions;
using MagicT.Server.BackgroundTasks;
using MagicT.Server.Helpers;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

 
//*** Important Note : Make sure server.crt and server.key copyToOutputDirectory property is set to Always copy
var crtPath = Path.Combine(Environment.CurrentDirectory, builder.Configuration.GetSection("Certificate:CrtPath").Value);
var keyPath = Path.Combine(Environment.CurrentDirectory, builder.Configuration.GetSection("Certificate:KeyPath").Value);

var certificate = CertificateHelper.GetCertificate(crtPath,keyPath);

#if DEBUG
//Verify certificate
var verf = certificate.Verify();
#endif
Console.WriteLine();


builder.WebHost.ConfigureKestrel((context, opt) =>
{
    opt.ListenLocalhost(7178, o =>
    {
        o.Protocols = HttpProtocols.Http1;
        o.UseHttps(certificate);
    });
});








// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddMagicOnion(x =>
{
    x.IsReturnExceptionStackTraceInErrorDetail = true;

    //x.EnableCurrentContext = true;
    x.MessageSerializer = MemoryPackMagicOnionSerializerProvider.Instance;
});


builder.Services.AddSingleton<DbExceptionHandler>();

builder.Services.AddDbContext<MagicTContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnection("DefaultConnection")));

builder.Services.AddHostedService<QueuedHostedService>();

builder.Services.AddSingleton<IBackGroundTaskQueue>(x =>
{
    var result = int.TryParse(builder.Configuration["QueueCapacity"], out int queueCapacity);

    if (!result) queueCapacity = 100;

    return new BackGroundTaskQueue(queueCapacity, builder.Services);
});

builder.Services.AddMessagePipe();

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

builder.Services.AddSingleton(provider => new MemoryDatabaseManager(provider));

//builder.Services.AddScoped<MemoryDatabaseInitializer>();

//builder.Services.AddSingleton(provider   => MemoryDatabaseInitializer.CreateMemoryDatabase());

var app = builder.Build();

//using var scope = app.Services.CreateAsyncScope();
app.Services.GetService<MemoryDatabaseManager>().CreateNewDatabase();

// Configure the HTTP request pipeline.

app.UseRouting();

app.MapMagicOnionHttpGateway("_", app.Services.GetService<MagicOnionServiceDefinition>().MethodHandlers,
    GrpcChannel.ForAddress("http://localhost:5029")); // Use HTTP instead of HTTPS
app.MapMagicOnionSwagger("swagger", app.Services.GetService<MagicOnionServiceDefinition>().MethodHandlers, "/_/");

app.MapMagicOnionService();

app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();