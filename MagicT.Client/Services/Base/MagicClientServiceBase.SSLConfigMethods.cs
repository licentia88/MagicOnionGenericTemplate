
using Grpc.Core;
using MagicOnion;
using MagicT.Shared.Models.ServiceModels;
#if (GRPC_SSL)
using Grpc.Net.Client;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
#endif

namespace MagicT.Client.Services.Base;

public abstract partial class MagicClientServiceBase<TService, TModel>
{
#if (GRPC_SSL)
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

    /// <summary>
    ///     Configures the service instance with a cancellation token.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The configured service instance.</returns>
    public virtual TService WithCancellationToken(CancellationToken cancellationToken)
    {
        return Client.WithCancellationToken(cancellationToken);
    }

    /// <summary>
    ///     Configures the service instance with a deadline.
    /// </summary>
    /// <param name="deadline">The deadline.</param>
    /// <returns>The configured service instance.</returns>
    public virtual TService WithDeadline(DateTime deadline)
    {
        return Client.WithDeadline(deadline);
    }

    /// <summary>
    ///     Configures the service instance with custom headers.
    /// </summary>
    /// <param name="headers">The headers.</param>
    /// <returns>The configured service instance.</returns>
    public virtual TService WithHeaders(Metadata headers)
    {
        return Client.WithHeaders(headers);
    }

    /// <summary>
    ///     Configures the service instance with a custom host.
    /// </summary>
    /// <param name="host">The host.</param>
    /// <returns>The configured service instance.</returns>
    public virtual TService WithHost(string host)
    {
        return Client.WithHost(host);
    }

    /// <summary>
    ///     Configures the service instance with custom call options.
    /// </summary>
    /// <param name="option">The call options.</param>
    /// <returns>The configured service instance.</returns>
    public virtual TService WithOptions(CallOptions option)
    {
        return Client.WithOptions(option);
    }

   
}

