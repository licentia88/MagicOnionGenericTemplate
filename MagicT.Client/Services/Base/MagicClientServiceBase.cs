using Grpc.Net.Client;
using MagicOnion;
using MagicOnion.Client;
using MagicOnion.Serialization.MemoryPack;
using MagicT.Shared.Models.ServiceModels;
using MagicT.Shared.Services.Base;
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
public abstract partial class MagicClientServiceBase<TService, TModel> : IMagicService<TService, TModel>
    where TService : IMagicService<TService, TModel> //, IService<TService>
{
    /// <summary>
    /// The client instance used to interact with the service.
    /// </summary>
    protected readonly TService Client;

    public IConfiguration Configuration { get; set; }


    public MagicClientServiceBase(IServiceProvider provider) : this(provider, default) { }
     
    /// <summary>
    ///     Initializes a new instance of the <see cref="MagicClientServiceBase{TService,TModel}" /> class.
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="filters"></param>
    protected MagicClientServiceBase(IServiceProvider provider, params IClientFilter[] filters)
    {
#if (GRPC_SSL)
        Configuration = provider.GetService<IConfiguration>();
        var configuration = provider.GetService<IConfiguration>();
        //Make sure certificate file's copytooutputdirectory is set to always copy
        var certificatePath = Path.Combine(Environment.CurrentDirectory, Configuration.GetSection("Certificate").Value);
        
        var certificate = new X509Certificate2(File.ReadAllBytes(certificatePath));

        var SslAuthOptions = CreateSslClientAuthOptions(certificate);

        var socketHandler = CreateHttpClientWithSocketsHandler(SslAuthOptions, Timeout.InfiniteTimeSpan, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(30));

        var channelOptions = CreateGrpcChannelOptions(socketHandler);

        var channel = GrpcChannel.ForAddress("https://localhost:7197", channelOptions);
#else
        var channel = GrpcChannel.ForAddress("http://localhost:5029"); 
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
}