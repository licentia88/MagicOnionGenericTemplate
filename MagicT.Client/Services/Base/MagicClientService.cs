using Blazored.LocalStorage;
using Grpc.Core;
using Grpc.Net.Client;
using MagicOnion;
using MagicOnion.Client;
using MagicOnion.Serialization.MemoryPack;
using MagicT.Shared.Models.ServiceModels;
using MagicT.Shared.Services.Base;
//using Majorsoft.Blazor.Extensions.BrowserStorage;
using Microsoft.Extensions.Configuration;
#if (GRPC_SSL)
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
#endif
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Client.Services.Base;


/// <summary>
///     Abstract base class for a generic service implementation.
/// </summary>
/// <typeparam name="TService">The type of service.</typeparam>
/// <typeparam name="TModel">The type of model.</typeparam>
public abstract partial class MagicClientService<TService, TModel> : IMagicService<TService, TModel>//,IService<TService>
    where TService : IMagicService<TService, TModel> 
{
    /// <summary>
    /// The client instance used to interact with the service.
    /// </summary>
    protected readonly TService Client;

    public IConfiguration Configuration { get; set; }

    public IServiceProvider Provider { get; set; }

    public ILocalStorageService LocalStorageService { get; set; }

    IConfigurationSection DockerConfig { get; set; }

    public MagicClientService(IServiceProvider provider) : this(provider, default)
    {
       


    }
     
    /// <summary>
    ///     Initializes a new instance of the <see cref="MagicClientService{TService,TModel}" /> class.
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="filters"></param>
    protected MagicClientService(IServiceProvider provider, params IClientFilter[] filters)
    {
        Provider = provider;

        LocalStorageService = provider.GetService<ILocalStorageService>();
        Configuration = provider.GetService<IConfiguration>();
        DockerConfig = Configuration.GetSection("DockerConfig");

#if GRPC_SSL
        string endpoint = "https://localhost:7197";
#else
        string endpoint = "http://localhost:5029";
#endif

        if (DockerConfig.GetValue<bool>("DockerBuild"))
        {
            endpoint = "http://magictserver";
        }
 
#if GRPC_SSL

        
        Configuration = provider.GetService<IConfiguration>();
        var configuration = provider.GetService<IConfiguration>();
        //Make sure certificate file's copytooutputdirectory is set to always copy
        var certificatePath = Path.Combine(Environment.CurrentDirectory, Configuration.GetSection("Certificate").Value);
        
        var certificate = new X509Certificate2(File.ReadAllBytes(certificatePath));

        var SslAuthOptions = CreateSslClientAuthOptions(certificate);

        var socketHandler = CreateHttpClientWithSocketsHandler(SslAuthOptions, Timeout.InfiniteTimeSpan, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(30));

        var channelOptions = CreateGrpcChannelOptions(socketHandler);

        var channel = GrpcChannel.ForAddress(endpoint, channelOptions);
#else
        var channel = GrpcChannel.ForAddress(endpoint); 
#endif

        Client = MagicOnionClient.Create<TService>(channel, MemoryPackMagicOnionSerializerProvider.Instance, filters);

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
    public virtual UnaryResult<List<TModel>> ReadAsync()
    {
        return Client.ReadAsync();
    }

    /// <summary>
    /// Initiates a server streaming operation to asynchronously retrieve a stream of model data in batches.
    /// </summary>
    /// <param name="batchSize">The number of items to retrieve in each batch.</param>
    /// <returns>A task representing the server streaming result containing a stream of model data.</returns>
    public virtual Task<ServerStreamingResult<List<TModel>>> StreamReadAllAsync(int batchSize)
    {
        return Client.StreamReadAllAsync(batchSize);
    }

 

    /// <summary>
    /// Creates a new encrypted data using the provided encrypted data.
    /// </summary>
    /// <param name="encryptedData">The encrypted data to create.</param>
    /// <returns>A unary result containing the created encrypted data.</returns>
    UnaryResult<EncryptedData<TModel>> IMagicService<TService,TModel>.CreateEncrypted(EncryptedData<TModel> encryptedData)
    {
        return Client.CreateEncrypted(encryptedData);
    }

    /// <summary>
    /// Reads all encrypted data.
    /// </summary>
    /// <returns>A unary result containing a list of encrypted data.</returns>
    UnaryResult<EncryptedData<List<TModel>>> IMagicService<TService,TModel>.ReadEncryptedAsync()
    {
        return Client.ReadEncryptedAsync();
    }

    /// <summary>
    /// Updates an encrypted data using the provided encrypted data.
    /// </summary>
    /// <param name="encryptedData">The encrypted data to update.</param>
    /// <returns>A unary result containing the updated encrypted data.</returns>
    UnaryResult<EncryptedData<TModel>> IMagicService<TService, TModel>.UpdateEncrypted(EncryptedData<TModel> encryptedData)
    {
        return Client.UpdateEncrypted(encryptedData);
    }

    /// <summary>
    /// Deletes an encrypted data using the provided encrypted data.
    /// </summary>
    /// <param name="encryptedData">The encrypted data to delete.</param>
    /// <returns>A unary result containing the deleted encrypted data.</returns>
    UnaryResult<EncryptedData<TModel>> IMagicService<TService, TModel>.DeleteEncryptedAsync(EncryptedData<TModel> encryptedData)
    {
        return Client.DeleteEncryptedAsync(encryptedData);
    }


    UnaryResult<EncryptedData<List<TModel>>> IMagicService<TService, TModel>.FindByParentEncryptedAsync(EncryptedData<string> parentId, EncryptedData<string> foreignKey)
    {
        return Client.FindByParentEncryptedAsync(parentId, foreignKey);
    }

    Task<ServerStreamingResult<EncryptedData<List<TModel>>>> IMagicService<TService, TModel>.StreamReadAllEncyptedAsync(int batchSize)
    {
        return Client.StreamReadAllEncyptedAsync(batchSize);
    }

    public UnaryResult<List<TModel>> FindByParametersAsync(byte[] parameters)
    {
        return Client.FindByParametersAsync(parameters);
    }

    public UnaryResult<EncryptedData<List<TModel>>> FindByParametersEncryptedAsync(EncryptedData<byte[]> parameterBytes)
    {
        return Client.FindByParametersEncryptedAsync(parameterBytes);
         
    }

    public IService<TService> WithHeader(Metadata data)
    {
        return Client.WithHeaders(data);
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