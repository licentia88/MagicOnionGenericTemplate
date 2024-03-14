using LitJWT;
using LitJWT.Algorithms;
using MagicOnion.Serialization.MemoryPack;
using MagicT.Server.Database;
using MagicT.Server.Jwt;
using Microsoft.EntityFrameworkCore;
using MagicT.Server.Exceptions;
using MagicT.Server.Initializers;
using MagicT.Shared.Extensions;
using Coravel;
using MagicT.Server.Invocables;
using MagicT.Server.Managers;
using MagicT.Shared.Managers;
using MagicT.Redis.Extensions;
using MagicOnion.Server;
using Grpc.Net.Client;

#if (SSL_CONFIG)
using MagicT.Server.Helpers;
#endif

var builder = WebApplication.CreateBuilder(args);

#if SSL_CONFIG
//*** Important Note : Make sure server.crt and server.key copyToOutputDirectory property is set to Always copy
var crtPath = Path.Combine(Environment.CurrentDirectory, builder.Configuration.GetSection("Certificate:CrtPath").Value);
var keyPath = Path.Combine(Environment.CurrentDirectory, builder.Configuration.GetSection("Certificate:KeyPath").Value);

var certificate = CertificateHelper.GetCertificate(crtPath,keyPath);
 
//Verify certificate
var verf = certificate.Verify();


//-:cnd
#if DEBUG

builder.WebHost.ConfigureKestrel(x =>
{
    x.ListenAnyIP(7197, o =>
    {
        o.Protocols = HttpProtocols.Http2;
        o.UseHttps(certificate);
    });

    x.ListenAnyIP(5029);
    x.ListenAnyIP(5028, (opt) => opt.Protocols = HttpProtocols.Http1);
});
#endif
//+:cnd

#else



#endif

 


// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc(x =>
{
    x.EnableDetailedErrors = true;
    x.MaxReceiveMessageSize =null; // 100 MB
    x.MaxSendMessageSize = null; // 100 MB
});


// Uncomment for HTTP1 Configuration

//builder.Services.AddCors(options =>
//{
//    options.AddDefaultPolicy(policy =>
//    {
//        // NOTE: "grpc-status" and "grpc-message" headers are required by gRPC. so, we need expose these headers to the client.
//        policy.WithExposedHeaders("grpc-status", "grpc-message");
//    });
//});

builder.Services.AddMagicOnion(x =>
{
//-:cnd
#if DEBUG
    x.IsReturnExceptionStackTraceInErrorDetail = true;
#endif
//+:cnd
    //Remove this line to use magic onion with message pack
    x.MessageSerializer = MemoryPackMagicOnionSerializerProvider.Instance;
   
});

builder.Services.RegisterShared(builder.Configuration);

builder.Services.AutoRegisterFromMagicTServer();
builder.Services.AutoRegisterFromMagicTShared();
builder.Services.AddSingleton<TokenManager>();

builder.Services.AddSingleton<AuthenticationManager>();

builder.Services.AddSingleton<AuditManager>();

builder.Services.AddSingleton<QueryManager>();

builder.Services.AddSingleton<FileTransferManager>();

builder.Services.AddScoped<CancellationTokenManager>();
 
builder.Services.AddSingleton<DbExceptionHandler>();

builder.Services.AddQueue();

builder.Services.AddTransient(typeof(AuditFailedInvocable<>));

builder.Services.AddTransient(typeof(AuditRecordsInvocable<>));

builder.Services.AddTransient(typeof(AuditQueryInvocable<>));

builder.Services.RegisterRedisDatabase();

builder.Services.AddDbContextPool<MagicTContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString(nameof(MagicTContext))!));

//builder.Services.AddSingleton<IAsyncRequestHandler<int, string>, MyAsyncRequestHandler>();

//builder.Services.AddSingleton<IKeyExchangeManager, KeyExchangeManager>();

//builder.Services.AddSingleton<KeyExchangeData>();

builder.Services.AddScoped<DataInitializer>();


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


var app = builder.Build();

using var scope = app.Services.CreateAsyncScope();
var KeyExchangeManager = app.Services.GetRequiredService<IKeyExchangeManager>();
KeyExchangeManager.Initialize();
scope.ServiceProvider.GetRequiredService<DataInitializer>().Initialize();


// Uncomment for HTTP1 Configuration

//app.UseCors();
//app.UseWebSockets();
//app.UseGrpcWebSocketRequestRoutingEnabler();



app.UseRouting();


app.UseGrpcWebSocketBridge();

var SwaggerUrl =  builder.Configuration.GetValue<string>("SwaggerUrl");

app.MapMagicOnionHttpGateway("api", app.Services.GetService<MagicOnionServiceDefinition>().MethodHandlers,
  GrpcChannel.ForAddress(SwaggerUrl)); // Use HTTP instead of HTTPS

app.MapMagicOnionSwagger("swagger", app.Services.GetService<MagicOnionServiceDefinition>().MethodHandlers, "/api/");


app.MapMagicOnionService();

app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");



//app.MapGet("/", (HttpContext context) => context.Request.Protocol.ToString());

app.Run();