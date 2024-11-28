using LitJWT;
using LitJWT.Algorithms;
using MagicOnion.Serialization.MemoryPack;
using MagicT.Server.Initializers;
using MagicT.Shared.Extensions;
using Coravel;
using MagicT.Server.Invocables;
using MagicT.Server.Managers;
using MagicT.Shared.Managers;
using MagicT.Redis.Extensions;
using MagicOnion.Server;
using Grpc.Net.Client;
using MagicT.Server.Interceptors;
using EntityFramework.Exceptions.SqlServer;
using System.Collections.Concurrent;
using MagicT.Shared.Helpers;


// Create a new WebApplication builder
var builder = WebApplication.CreateBuilder(args);

// Check if the environment is Development and the platform is not Windows
if (builder.Environment.IsDevelopment() && !PlatFormHelper.IsWindows())
{
    // Remove the HTTPS section if the platform is not Windows
    var config = builder.Configuration.GetSection("Kestrel:Endpoints");
    var endpoints = config.GetChildren().ToList();
    var httpsEndpoint = endpoints.FirstOrDefault(e => e.Key == "HTTPS");

    if (httpsEndpoint != null)
    {
        // Get the configuration as an enumerable list
        var configList = builder.Configuration.AsEnumerable().ToList();
        // Remove all entries related to the HTTPS endpoint
        configList.RemoveAll(x => x.Key.Contains("Kestrel:Endpoints:HTTPS"));
        // Clear the existing configuration sources
        builder.Configuration.Sources.Clear();
        // Add the modified configuration back to the builder
        builder.Configuration.AddInMemoryCollection(configList);
    }
}
 
#if SSL_CONFIG
//*** Important Note : Make sure server.crt and server.key copyToOutputDirectory property is set to Always copy
//var crtPath = Path.Combine(Environment.CurrentDirectory, builder.Configuration.GetSection("Certificate:CrtPath").Value);
//var keyPath = Path.Combine(Environment.CurrentDirectory, builder.Configuration.GetSection("Certificate:KeyPath").Value);

//var certificate = CertificateHelper.GetCertificate(crtPath,keyPath);
 
//Verify certificate
//var verf = certificate.Verify();

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
builder.Services.AutoRegisterFromMagicTRedis();

 

builder.Services.AddQueue();

builder.Services.AddSingleton(typeof(ConcurrentDictionary<,>));
 
// AutoregisterInject can not handle the generic types, thus we need to manually register these types manually
builder.Services.AddTransient(typeof(AuditQueryInvocable<>));
builder.Services.AddTransient(typeof(AuditFailedInvocable<>));
builder.Services.AddTransient(typeof(AuditRecordsInvocable<>));
builder.Services.AddSingleton(typeof(LogManager<>));


builder.Services.RegisterRedisDatabase();


// builder.Services.AddDbContext<MagicTContext>(
//     options => options.UseMySql(
//         builder.Configuration.GetConnectionString(nameof(MagicTContext))!,
//         ServerVersion.AutoDetect(builder.Configuration.GetConnectionString(nameof(MagicTContext)))
//     ).UseExpressionify(o => o.WithEvaluationMode(ExpressionEvaluationMode.FullCompatibilityButSlow))
// );
builder.Services.AddDbContext<MagicTContext>((sp, options) =>
  options.UseSqlServer(builder.Configuration.GetConnectionString(nameof(MagicTContext))!)
  .UseExceptionProcessor()
  .EnableSensitiveDataLogging()
  .AddInterceptors(sp.GetRequiredService<DbExceptionsInterceptor>())
  .LogTo(Console.WriteLine, LogLevel.None));


// Register the HS256Algorithm key generation and encoder/decoder
builder.Services.AddSingleton(_ =>
{
    var key = HS512Algorithm.GenerateRandomRecommendedKey();
    // var key = HS256Algorithm.GenerateRandomRecommendedKey();
    return new JwtEncoder(new HS512Algorithm(key));
});

builder.Services.AddSingleton<JwtDecoder>(provider =>
{
    var encoder = provider.GetRequiredService<JwtEncoder>();
    return new JwtDecoder(encoder.SignAlgorithm);
});


var app = builder.Build();

using var scope = app.Services.CreateAsyncScope();
app.Services.GetRequiredService<IKeyExchangeManager>();

Task.Run(() => scope.ServiceProvider.GetRequiredService<DataInitializer>().Initialize());

 

app.UseRouting();


var swaggerUrl = builder.Configuration.GetValue<string>(PlatFormHelper.IsWindows() 
    ? "Kestrel:Endpoints:HTTPS:Url" : "Kestrel:Endpoints:HTTP:Url");


app.MapMagicOnionHttpGateway("api", app.Services.GetService<MagicOnionServiceDefinition>().MethodHandlers,
  GrpcChannel.ForAddress(swaggerUrl)); // Use HTTP instead of HTTPS

app.MapMagicOnionSwagger("swagger", app.Services.GetService<MagicOnionServiceDefinition>().MethodHandlers, "/api/");


app.MapMagicOnionService();

app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

try
{
    app.Run();
}
catch (Exception ex)
{
    // ignored
}