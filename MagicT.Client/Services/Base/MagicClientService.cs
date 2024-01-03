using Grpc.Core;
using MagicOnion;
using MagicOnion.Client;
using MagicT.Client.Extensions;
using MagicT.Client.Filters;
using MagicT.Shared.Models.ServiceModels;
using MagicT.Shared.Services.Base;


namespace MagicT.Client.Services.Base;

/// <summary>
///     Abstract base class for a generic service implementation.
/// </summary>
/// <typeparam name="TService">The type of service.</typeparam>
/// <typeparam name="TModel">The type of model.</typeparam>
public abstract class MagicClientService<TService, TModel> : MagicClientServiceBase<TService>, IMagicService<TService, TModel>
    where TService : IMagicService<TService, TModel> 
{

    public IClientFilter[] Filters { get; set; } = default;

    protected MagicClientService(IServiceProvider provider) : base(provider)
    {
    }

    protected MagicClientService(IServiceProvider provider, params IClientFilter[] filters) : base(provider, filters)
    {
        Filters = filters;
    }



    /// <summary>
    ///     Creates a new instance of the specified model.
    /// </summary>
    /// <param name="model">The model to create.</param>
    /// <returns>A unary result containing the created model.</returns>
    public virtual UnaryResult<TModel> CreateAsync(TModel model)
    {
        return Client.CreateAsync(model);
    }


    /// <summary>
    /// Retrieves a list of entities of type TModel associated with a parent entity based on a foreign key.
    /// </summary>
    /// <param name="parentId">The identifier of the parent entity.</param>
    /// <param name="foreignKey">The foreign key used to associate the entities with the parent entity.</param>
    /// <returns>A unary result containing the list of associated entities.</returns>
    public virtual UnaryResult<List<TModel>> FindByParentAsync(string parentId, string foreignKey)
    {
        return Client.FindByParentAsync(parentId, foreignKey);
    }


    /// <summary>
    ///     Updates the specified model.
    /// </summary>
    /// <param name="model">The model to update.</param>
    /// <returns>A unary result containing the updated model.</returns>
    public virtual UnaryResult<TModel> UpdateAsync(TModel model)
    {
        return Client.UpdateAsync(model);
    }

    /// <summary>
    ///     Deletes the specified model.
    /// </summary>
    /// <param name="model">The model to delete.</param>
    /// <returns>A unary result containing the deleted model.</returns>
    public virtual UnaryResult<TModel> DeleteAsync(TModel model)
    {
        return Client.DeleteAsync(model);
    }

    /// <summary>
    ///     Retrieves all models.
    /// </summary>
    /// <returns>A unary result containing a list of all models.</returns>
    public virtual async UnaryResult<List<TModel>> ReadAsync()
    {
        var result = await Client.ReadAsync();

        return result;
    }

    /// <summary>
    /// Initiates a server streaming operation to asynchronously retrieve a stream of model data in batches.
    /// </summary>
    /// <param name="batchSize">The number of items to retrieve in each batch.</param>
    /// <returns>A task representing the server streaming result containing a stream of model data.</returns>
    public virtual async Task<ServerStreamingResult<List<TModel>>> StreamReadAllAsync(int batchSize)
    {
        
        
        return await Client.StreamReadAllAsync(batchSize);
    }
 
    public UnaryResult<List<TModel>> FindByParametersAsync(byte[] parameters)
    {
        return Client.FindByParametersAsync(parameters);
    }

 
    //public TService AddHubKey<THub>() where THub :IHubConnection
    //{
    //    var hub = Provider.GetService<THub>();

    //    var id = hub.GetConnectionId();

    //    return Client;
    //    //Client.WithOptions(new Grpc.Core.CallOptions { Headers = header})
    //}

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


  


}