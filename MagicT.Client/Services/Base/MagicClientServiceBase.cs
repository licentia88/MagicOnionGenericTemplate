using Blazored.LocalStorage;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcWebSocketBridge.Client;
using MagicOnion;
using MagicOnion.Client;
using MagicOnion.Serialization.MemoryPack;
using Microsoft.Extensions.Configuration;
#if (SSL_CONFIG)
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
#endif
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Client.Services.Base;

public abstract class MagicClientServiceBase<TService>: IService<TService> where TService : IService<TService>
{
    protected readonly TService Client;

    public IConfiguration Configuration { get; set; }

    public IServiceProvider Provider { get; set; }

    public ILocalStorageService LocalStorageService { get; set; }

    public MagicClientServiceBase(IServiceProvider provider) : this(provider, default)
    {

    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MagicClientService{TService,TModel}" /> class.
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="filters"></param>
    protected MagicClientServiceBase(IServiceProvider provider, params IClientFilter[] filters)
    {
        Provider = provider;

        LocalStorageService = provider.GetService<ILocalStorageService>();
        Configuration = provider.GetService<IConfiguration>();


#if (SSL_CONFIG)
        string baseUrl = Configuration.GetValue<string>("API_BASE_URL:HTTPS");
#else
        string baseUrl = Configuration.GetValue<string>("API_BASE_URL:HTTP");
#endif


#if (SSL_CONFIG)

        
        Configuration = provider.GetService<IConfiguration>();
        var configuration = provider.GetService<IConfiguration>();
        //Make sure certificate file's copytooutputdirectory is set to always copy
        var certificatePath = Path.Combine(Environment.CurrentDirectory, Configuration.GetSection("Certificate").Value);
        
        var certificate = new X509Certificate2(File.ReadAllBytes(certificatePath));

        var SslAuthOptions = CreateSslClientAuthOptions(certificate);

        var socketHandler = CreateHttpClientWithSocketsHandler(SslAuthOptions, Timeout.InfiniteTimeSpan, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(30));

        var channelOptions = CreateGrpcChannelOptions(socketHandler);

        var channel = GrpcChannel.ForAddress(baseUrl, channelOptions);
#else
        var channel = GrpcChannel.ForAddress(baseUrl);



#endif

        // Uncomment for HTTP1 Configuration

        channel = GrpcChannel.ForAddress(baseUrl, new GrpcChannelOptions()
        {
            HttpHandler = new GrpcWebSocketBridgeHandler(true)
        });

        Client = MagicOnionClient.Create<TService>(channel, MemoryPackMagicOnionSerializerProvider.Instance, filters);

    }

    public virtual TService WithOptions(CallOptions option)
    {
        return Client.WithOptions(option);
    }

    public virtual TService WithHeaders(Metadata headers)
    {
        return Client.WithHeaders(headers);
    }

    public virtual TService WithDeadline(DateTime deadline)
    {
        return Client.WithDeadline(deadline);
    }

    public virtual TService WithCancellationToken(CancellationToken cancellationToken)
    {
        return Client.WithCancellationToken(cancellationToken);
    }

    public virtual TService WithHost(string host)
    {
        return Client.WithHost(host);
    }

#if (SSL_CONFIG)
    /// <summary>
    /// Creates SSL client authentication options for gRPC client communication.
    /// </summary>
    /// <param name="certificate">The X509 certificate used for client authentication.</param>
    /// <returns>An instance of <see cref="SslClientAuthenticationOptions"/> configured with the certificate and validation callback.</returns>
    public SslClientAuthenticationOptions CreateSslClientAuthOptions(X509Certificate2 certificate)
    {
        return new SslClientAuthenticationOptions
        {
            RemoteCertificateValidationCallback = (sender, cert, _, _) =>
            {
                X509Chain x509Chain = new X509Chain();
                x509Chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                bool isChainValid = x509Chain.Build(new X509Certificate2(cert));
                return isChainValid;
            },
            ClientCertificates = new X509Certificate2Collection { certificate }
        };
    }


    /// <summary>
    /// Creates an instance of <see cref="SocketsHttpHandler"/> configured with provided options.
    /// </summary>
    /// <param name="sslClientAuthenticationOptions">The SSL client authentication options.</param>
    /// <param name="pooledConnectionIdleTimeout">The timeout for idle pooled connections.</param>
    /// <param name="keepAlivePingDelay">The delay between keep-alive pings.</param>
    /// <param name="keepAlivePingTimeout">The timeout for keep-alive pings.</param>
    /// <param name="enableMultipleHttp2Connections">Indicates whether multiple HTTP/2 connections are enabled.</param>
    /// <returns>An instance of <see cref="SocketsHttpHandler"/> configured with the provided options.</returns>
    public SocketsHttpHandler CreateHttpClientWithSocketsHandler(SslClientAuthenticationOptions sslClientAuthenticationOptions,
     TimeSpan pooledConnectionIdleTimeout,
     TimeSpan keepAlivePingDelay,
     TimeSpan keepAlivePingTimeout,
     bool enableMultipleHttp2Connections = true)
    {
        var socketsHandler = new SocketsHttpHandler
        {
            SslOptions = sslClientAuthenticationOptions,
            PooledConnectionIdleTimeout = pooledConnectionIdleTimeout,
            KeepAlivePingDelay = keepAlivePingDelay,
            KeepAlivePingTimeout = keepAlivePingTimeout,
            EnableMultipleHttp2Connections = enableMultipleHttp2Connections
        };

        return socketsHandler;
    }

    /// <summary>
    /// Creates gRPC channel options with the provided <see cref="SocketsHttpHandler"/>.
    /// </summary>
    /// <param name="socketsHandler">The configured <see cref="SocketsHttpHandler"/>.</param>
    /// <returns>An instance of <see cref="GrpcChannelOptions"/> with the specified HTTP handler and message size limits.</returns>
    public GrpcChannelOptions CreateGrpcChannelOptions(SocketsHttpHandler socketsHandler)
    {
        var channelOptions = new GrpcChannelOptions
        {
            HttpHandler = socketsHandler,
            MaxReceiveMessageSize = null,
            MaxSendMessageSize = null
        };

        return channelOptions;
    }
#endif
}
