using Grpc.Net.Client;
using LitJWT;
using LitJWT.Algorithms;
using Magic.Server.Database;
using Magic.Server.Extensions;
using Magic.Server.Jwt;
using MagicOnion.Serialization.MemoryPack;
using MagicOnion.Server;
using MessagePipe;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddMagicOnion(x => {
    x.IsReturnExceptionStackTraceInErrorDetail = true;
    //x.EnableCurrentContext = true;
    x.MessageSerializer = MemoryPackMagicOnionSerializerProvider.Instance;
});

builder.Services.AddDbContext<DummyContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnection("DefaultConnection")));

builder.Services.AddMessagePipe();
builder.Services.AddSingleton(x =>
{
    var key = HS256Algorithm.GenerateRandomRecommendedKey();

    var encoder = new JwtEncoder(new HS256Algorithm(key));
    var decoder = new JwtDecoder(encoder.SignAlgorithm);

    return new FastJwtTokenService
    {
        Encoder = encoder,
        Decoder = decoder
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseRouting();

app.MapMagicOnionHttpGateway("_", app.Services.GetService<MagicOnionServiceDefinition>().MethodHandlers, GrpcChannel.ForAddress("http://localhost:5002")); // Use HTTP instead of HTTPS
app.MapMagicOnionSwagger("swagger", app.Services.GetService<MagicOnionServiceDefinition>().MethodHandlers, "/_/");

app.MapMagicOnionService();


app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();

