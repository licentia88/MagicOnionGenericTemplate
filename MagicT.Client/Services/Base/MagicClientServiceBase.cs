using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Benutomo;
using Blazored.LocalStorage;
using Grpc.Core;
using Grpc.Net.Client;
using MagicOnion;
using MagicOnion.Client;
using MagicOnion.Serialization.MemoryPack;
using MagicT.Shared.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Client.Services.Base
{
    /// <summary>
    /// Represents the base class for Magic Client Services, providing common functionality for gRPC client communication.
    /// </summary>
    /// <typeparam name="TService">The type of the service interface.</typeparam>
    [AutomaticDisposeImpl]
    public abstract partial class MagicClientServiceBase<TService> : IService<TService>, IDisposable where TService : IService<TService>
    {
        /// <summary>
        /// Gets or sets a value indicating whether to use SSL for gRPC communication.
        /// </summary>
        protected bool UseSsl { get; }

        /// <summary>
        /// Gets the gRPC client instance.
        /// </summary>
        protected TService Client;

        /// <summary>
        /// Gets or sets the configuration instance.
        /// </summary>
        private IConfiguration Configuration { get; }

        /// <summary>
        /// Gets or sets the local storage service instance.
        /// </summary>
        protected ILocalStorageService LocalStorageService { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MagicClientServiceBase{TService}"/> class with the specified service provider.
        /// </summary>
        /// <param name="provider">The service provider.</param>
        protected MagicClientServiceBase(IServiceProvider provider) : this(provider, default)
        {
        }

        ~MagicClientServiceBase()
        {
            Dispose(false);
        }
        /// <summary>
        /// Gets or sets the base URL for the gRPC channel.
        /// </summary>
        public string BaseUrl { get; set; }
        
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MagicClientServiceBase{TService}"/> class with the specified service provider and client filters.
        /// </summary>
        /// <param name="provider">The service provider.</param>
        /// <param name="filters">The client filters.</param>
        protected MagicClientServiceBase(IServiceProvider provider, params IClientFilter[] filters)
        {
            LocalStorageService = provider.GetService<ILocalStorageService>();
            Configuration = provider.GetService<IConfiguration>();

            // Initialize UseSSL based on the presence of the SSL_CONFIG preprocessor directive
#if (SSL_CONFIG)
            UseSsl = true;
#else
            UseSsl = false;
#endif

            BaseUrl = Configuration.GetValue<string>(UseSsl && PlatFormHelper.IsWindows()
                ? "API_BASE_URL:HTTPS" : "API_BASE_URL:HTTP");

            // ReSharper disable once VirtualMemberCallInConstructor
            SetBaseUrl();
            GrpcChannel channel = CreateChannel(BaseUrl);

            Client = MagicOnionClient.Create<TService>(channel, MemoryPackMagicOnionSerializerProvider.Instance, filters);

           
        }

        /// <summary>
        /// Sets the base URL for the gRPC channel. Can be overridden by derived classes to set a different base URL.
        /// </summary>
        protected virtual void SetBaseUrl()
        {
            // Default implementation does nothing, derived classes can override this method
        }
        
        
         

        /// <summary>
        /// Creates a new instance of the service with the specified call options.
        /// </summary>
        /// <param name="option">The call options.</param>
        /// <returns>A new instance of the service with the specified call options.</returns>
        public virtual TService WithOptions(CallOptions option)
        {
            return Client.WithOptions(option);
        }

        /// <summary>
        /// Creates a new instance of the service with the specified headers.
        /// </summary>
        /// <param name="headers">The headers.</param>
        /// <returns>A new instance of the service with the specified headers.</returns>
        public virtual TService WithHeaders(Metadata headers)
        {
            return Client.WithHeaders(headers);
        }

        /// <summary>
        /// Creates a new instance of the service with the specified deadline.
        /// </summary>
        /// <param name="deadline">The deadline.</param>
        /// <returns>A new instance of the service with the specified deadline.</returns>
        public virtual TService WithDeadline(DateTime deadline)
        {
            return Client.WithDeadline(deadline);
        }

        /// <summary>
        /// Creates a new instance of the service with the specified cancellation token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A new instance of the service with the specified cancellation token.</returns>
        public virtual TService WithCancellationToken(CancellationToken cancellationToken)
        {
            return Client.WithCancellationToken(cancellationToken);
        }

        /// <summary>
        /// Creates a new instance of the service with the specified host.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <returns>A new instance of the service with the specified host.</returns>
        public virtual TService WithHost(string host)
        {
            return Client.WithHost(host);
        }

        /// <summary>
        /// Creates a gRPC channel based on SSL configuration.
        /// </summary>
        /// <param name="baseUrl">The base URL for the gRPC channel.</param>
        /// <returns>A configured gRPC channel.</returns>
        private GrpcChannel CreateChannel(string baseUrl)
        {
            var channelOptions = new GrpcChannelOptions
            {
                MaxReceiveMessageSize = null,
                MaxSendMessageSize = null
            };

            if (!UseSsl || !PlatFormHelper.IsWindows()) 
                return GrpcChannel.ForAddress(baseUrl, channelOptions);
            
            var sslAuthOptions = ConfigureSslClientAuthOptions();
            var socketHandler = ConfigureHttpClientWithSocketsHandler(sslAuthOptions, Timeout.InfiniteTimeSpan, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(30));
            channelOptions.HttpHandler = socketHandler;

            return GrpcChannel.ForAddress(baseUrl, channelOptions);
        }

        /// <summary>
        /// Configures SSL client authentication options.
        /// </summary>
        /// <param name="certificate">The optional certificate for SSL authentication.</param>
        /// <returns>An instance of <see cref="SslClientAuthenticationOptions"/> configured with the provided certificate.</returns>
        private SslClientAuthenticationOptions ConfigureSslClientAuthOptions(X509Certificate2 certificate = null)
        {
            return new SslClientAuthenticationOptions
            {
                RemoteCertificateValidationCallback = (sender, cert, chain, errors) =>
                {
                    if (errors == SslPolicyErrors.None)
                        return true;

                    if (certificate == null)
                        return false;

                    var x509Chain = new X509Chain
                    {
                        ChainPolicy = { RevocationMode = X509RevocationMode.NoCheck }
                    };

                    return cert != null && x509Chain.Build(new X509Certificate2(cert));
                },
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
        private SocketsHttpHandler ConfigureHttpClientWithSocketsHandler(SslClientAuthenticationOptions sslClientAuthenticationOptions,
            TimeSpan pooledConnectionIdleTimeout,
            TimeSpan keepAlivePingDelay,
            TimeSpan keepAlivePingTimeout,
            bool enableMultipleHttp2Connections = true)
        {
            return new SocketsHttpHandler
            {
                SslOptions = sslClientAuthenticationOptions,
                PooledConnectionIdleTimeout = pooledConnectionIdleTimeout,
                KeepAlivePingDelay = keepAlivePingDelay,
                KeepAlivePingTimeout = keepAlivePingTimeout,
                EnableMultipleHttp2Connections = enableMultipleHttp2Connections
            };
        }
    }
}