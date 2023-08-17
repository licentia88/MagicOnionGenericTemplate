using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using MagicOnion;
using MagicOnion.Client;
using MagicOnion.Serialization.MemoryPack;
using MagicT.Shared.Models.ServiceModels;
using MagicT.Shared.Services.Base;
using Org.BouncyCastle.Asn1.X509;

namespace MagicT.Client.Services.Base;

 
/// <summary>
///     Abstract base class for a generic service implementation.
/// </summary>
/// <typeparam name="TService">The type of service.</typeparam>
/// <typeparam name="TModel">The type of model.</typeparam>
public abstract class MagicTClientServiceBase<TService, TModel> : IMagicTService<TService, TModel>
    where TService : IMagicTService<TService, TModel> //, IService<TService>
{
    /// <summary>
    /// The client instance used to interact with the service.
    /// </summary>
    protected readonly TService Client;

    private static SslClientAuthenticationOptions GetSslClientAuthenticationOptions(X509Certificate2 certificate2)
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
            ClientCertificates = new X509Certificate2Collection { certificate2 }
        };
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MagicTClientServiceBase{TService,TModel}" /> class.
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="filters"></param>
    protected MagicTClientServiceBase(IServiceProvider provider, params IClientFilter[] filters)
    {
        var certificate = X509Certificate2.CreateFromPemFile("/Users/asimgunduz/server.crt", Path.ChangeExtension("/Users/asimgunduz/server.crt", "key"));

        var socketsHandler = new SocketsHttpHandler
        {
            SslOptions = GetSslClientAuthenticationOptions(certificate),
            PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
            KeepAlivePingDelay = TimeSpan.FromSeconds(60),
            KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
            EnableMultipleHttp2Connections = true
        };

        //var hand = new GrpcWebHandler(GrpcWebMode.GrpcWeb, socketsHandler) ;

        //hand.HttpVersion = HttpVersion.Version11;

        // Create an SSL credentials object
        //var sslCreds = new SslCredentials(File.ReadAllText("/Users/asimgunduz/server.crt"));

        // Create channel options and use the SSL credentials
        var channelOptions = new GrpcChannelOptions { HttpHandler= socketsHandler };

        // Create the channel with SSL credentials 
        var channel = GrpcChannel.ForAddress("https://localhost:7197", channelOptions);
     
         Client = MagicOnionClient.Create<TService>(channel, MemoryPackMagicOnionSerializerProvider.Instance, filters);
         
        //Client = Client.WithOptions(SenderOption);
    }



    /// <summary>
    ///     Creates a new instance of the specified model.
    /// </summary>
    /// <param name="model">The model to create.</param>
    /// <returns>A unary result containing the created model.</returns>
    public virtual UnaryResult<TModel> Create(TModel model)
    {
        return Client.Create(model);
    }


    /// <summary>
    /// Retrieves a list of entities of type TModel associated with a parent entity based on a foreign key.
    /// </summary>
    /// <param name="parentId">The identifier of the parent entity.</param>
    /// <param name="foreignKey">The foreign key used to associate the entities with the parent entity.</param>
    /// <returns>A unary result containing the list of associated entities.</returns>
    public virtual UnaryResult<List<TModel>> FindByParent(string parentId, string foreignKey)
    {
        return Client.FindByParent(parentId, foreignKey);
    }


    /// <summary>
    ///     Updates the specified model.
    /// </summary>
    /// <param name="model">The model to update.</param>
    /// <returns>A unary result containing the updated model.</returns>
    public virtual UnaryResult<TModel> Update(TModel model)
    {
        return Client.Update(model);
    }

    /// <summary>
    ///     Deletes the specified model.
    /// </summary>
    /// <param name="model">The model to delete.</param>
    /// <returns>A unary result containing the deleted model.</returns>
    public virtual UnaryResult<TModel> Delete(TModel model)
    {
        return Client.Delete(model);
    }

    /// <summary>
    ///     Retrieves all models.
    /// </summary>
    /// <returns>A unary result containing a list of all models.</returns>
    public virtual UnaryResult<List<TModel>> ReadAll()
    {
        return Client.ReadAll();
    }

    /// <summary>
    /// Initiates a server streaming operation to asynchronously retrieve a stream of model data in batches.
    /// </summary>
    /// <param name="batchSize">The number of items to retrieve in each batch.</param>
    /// <returns>A task representing the server streaming result containing a stream of model data.</returns>
    public virtual Task<ServerStreamingResult<List<TModel>>> StreamReadAll(int batchSize)
    {
        return Client.StreamReadAll(batchSize);
    }


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


    /// <summary>
    /// Creates a new encrypted data using the provided encrypted data.
    /// </summary>
    /// <param name="encryptedData">The encrypted data to create.</param>
    /// <returns>A unary result containing the created encrypted data.</returns>
    UnaryResult<EncryptedData<TModel>> IMagicTService<TService,TModel>.CreateEncrypted(EncryptedData<TModel> encryptedData)
    {
        return Client.CreateEncrypted(encryptedData);
    }

    /// <summary>
    /// Reads all encrypted data.
    /// </summary>
    /// <returns>A unary result containing a list of encrypted data.</returns>
    public UnaryResult<EncryptedData<List<TModel>>> ReadAllEncrypted()
    {
        return Client.ReadAllEncrypted();
    }

    /// <summary>
    /// Updates an encrypted data using the provided encrypted data.
    /// </summary>
    /// <param name="encryptedData">The encrypted data to update.</param>
    /// <returns>A unary result containing the updated encrypted data.</returns>
    UnaryResult<EncryptedData<TModel>> IMagicTService<TService, TModel>.UpdateEncrypted(EncryptedData<TModel> encryptedData)
    {
        return Client.UpdateEncrypted(encryptedData);
    }

    /// <summary>
    /// Deletes an encrypted data using the provided encrypted data.
    /// </summary>
    /// <param name="encryptedData">The encrypted data to delete.</param>
    /// <returns>A unary result containing the deleted encrypted data.</returns>
    UnaryResult<EncryptedData<TModel>> IMagicTService<TService, TModel>.DeleteEncrypted(EncryptedData<TModel> encryptedData)
    {
        return Client.DeleteEncrypted(encryptedData);
    }

 
}