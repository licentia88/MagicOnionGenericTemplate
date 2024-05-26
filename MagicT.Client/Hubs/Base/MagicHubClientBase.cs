using Grpc.Net.Client;
using MagicOnion.Client;
using MagicOnion.Serialization.MemoryPack;
using MagicT.Shared.Enums;
using MagicT.Shared.Hubs.Base;
using MessagePipe;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using GrpcWebSocketBridge.Client;
using MagicT.Shared.Extensions;


#if (SSL_CONFIG)
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
#endif

namespace MagicT.Client.Hubs.Base;

 
/// <summary>
/// Abstract base class for a generic service client implementation.
/// </summary>
/// <typeparam name="THub">The hub interface for the service.</typeparam>  
/// <typeparam name="TReceiver">The receiver interface for the client.</typeparam>
/// <typeparam name="TModel">The model type for the service.</typeparam>
public abstract partial class MagicHubClientBase<THub, TReceiver, TModel> : IMagicReceiver<TModel>, IMagicHub<THub, TReceiver, TModel>
    where THub : IMagicHub<THub, TReceiver, TModel>
    where TReceiver : class, IMagicReceiver<TModel>
{
    /// <summary>
    /// Publisher for individual model operations.
    /// </summary>
    private IPublisher<Operation, TModel> ModelPublisher { get; }

    /// <summary> 
    /// Publisher for bulk model operations.
    /// </summary>
    private IPublisher<Operation, List<TModel>> ListPublisher { get; }
    
    /// <summary>
    /// The collection of models on the client. 
    /// Updated by the receiver methods.
    /// </summary>
    public List<TModel> Collection { get; set; }
    
    
    /// <summary>
    /// The client used to interact with the service.
    /// </summary>
    protected THub Client;


    /// <summary>
    /// Configuration
    /// </summary>
    private IConfiguration Configuration { get; }

    private Timer _timer;
    /// <summary>
    /// Creates a new instance of the client.
    /// </summary>
    /// <param name="provider">The dependency injection provider.</param>
    public MagicHubClientBase(IServiceProvider provider)
    {
        Configuration = provider.GetService<IConfiguration>();
        //LocalStorageService = provider.GetService<ILocalStorageService>();
        Collection = provider.GetService<List<TModel>>();
        ModelPublisher = provider.GetService<IPublisher<Operation, TModel>>();
        ListPublisher = provider.GetService<IPublisher<Operation, List<TModel>>>();
    }
 
    /// <summary>
    /// Connects to the service hub.
    /// </summary>
    /// <returns>A task representing the async operation.</returns>
    public virtual async Task<Guid> ConnectAsync()
    {

#if (SSL_CONFIG)
        string baseUrl = Configuration.GetValue<string>("API_BASE_URL:HTTPS");
#else
        string baseUrl = Configuration.GetValue<string>("API_BASE_URL:HTTP");
#endif


#if (SSL_CONFIG)
        //Make sure certificate file's copytooutputdirectory is set to always copy
        var certificatePath = Path.Combine(Environment.CurrentDirectory, Configuration.GetSection("Certificate").Value);
        
        var certificate = new X509Certificate2(File.ReadAllBytes(certificatePath));

        var SslAuthOptions = CreateSslClientAuthOptions();

        var socketHandler = CreateHttpClientWithSocketsHandler(SslAuthOptions, Timeout.InfiniteTimeSpan, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(30));

        var channelOptions = CreateGrpcChannelOptions(socketHandler);

        var channel = GrpcChannel.ForAddress(baseUrl, channelOptions);
#else
        var channel = GrpcChannel.ForAddress(baseUrl, new GrpcChannelOptions
        {
            MaxReceiveMessageSize = null, // 5 MB
            MaxSendMessageSize = null// 2 MB
        });
#endif

        // Uncomment for HTTP1 Configuration

        //channel = GrpcChannel.ForAddress(baseUrl, new GrpcChannelOptions()
        //{
        //    HttpHandler = new GrpcWebSocketBridgeHandler()
        //});

        Client = await StreamingHubClient.ConnectAsync<THub, TReceiver>(
            channel, this as TReceiver, null,default, MemoryPackMagicOnionSerializerProvider.Instance);

        var connectionId = await Client.ConnectAsync();

        _timer = new Timer(KeepAliveAsync, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

        return connectionId;
    }

    /// <summary>
    /// This method is to keep the connection open
    /// </summary>
    /// <param name="state"></param>
    public async void KeepAliveAsync(object state)
    {
        await KeepAliveAsync();
    }
    /// <summary>
    /// Called when a new model is created on the service side.
    /// </summary>
    /// <param name="model">The new model.</param>
    public void OnCreate(TModel model)
    {
        Collection.Add(model);

        ModelPublisher.Publish(Operation.Create, model);
    }

    /// <summary>
    /// Called when a full model collection is read from the service.
    /// </summary>
    /// <param name="collection">The full collection from the service.</param> 
    void IMagicReceiver<TModel>.OnRead(List<TModel> collection)
    {
        //var uniqueData = collection.Except(Collection).ToList();

        Collection.AddRange(collection);

        ListPublisher.Publish(Operation.Read, Collection);
    }

    /// <summary>
    /// Called when a batch of models is streamed from the service.
    /// </summary>
    /// <param name="collection">The batch of models streamed.</param>
    void IMagicReceiver<TModel>.OnStreamRead(List<TModel> collection)
    {
        Collection.AddRange(collection);

        ListPublisher.Publish(Operation.Stream, collection);
    }

    /// <summary>
    /// Called when a model is updated on the service side. 
    /// </summary>
    /// <param name="model">The updated model.</param>
    public void OnUpdate(TModel model)
    {
        var index = Collection.IndexByKey(model);

        Collection[index] = model;

        ModelPublisher.Publish(Operation.Update, model);
    }

    /// <summary>
    /// Called when a model is deleted on the service side.
    /// </summary>
    /// <param name="model">The model that was deleted.</param>
    public void OnDelete(TModel model)
    {
        var index = Collection.IndexByKey(model);

        Collection.RemoveAt(index);

        ModelPublisher.Publish(Operation.Delete, model);
    }

    /// <summary>
    /// Called when the entire model collection has changed on the service side. 
    /// </summary>
    /// <param name="collection">The updated collection from the service.</param>
    public void OnCollectionChanged(List<TModel> collection)
    {
        Collection.Clear();
        Collection.AddRange(collection);

        ListPublisher.Publish(Operation.Read, Collection);
    }


    /// <summary>
    /// Requests the service to stream data.
    /// </summary>
    /// <param name="batchSize">The number of models per batch.</param>
    /// <returns>A task representing the async operation.</returns>
    public async Task StreamReadAsync(int batchSize)
    {
        await Client.StreamReadAsync(batchSize);
    }


    /// <summary>
    /// Requests the service to create a model.
    /// </summary>
    /// <param name="model">The model to create.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<TModel> CreateAsync(TModel model)
    {
        return  await Client.CreateAsync(model);
    }

    
    /// <summary>
    /// Requests the service to retrieve all models.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the response from the service, including the model list if successful.
    /// </returns>
    public Task ReadAsync()
    {
        return Client.ReadAsync();
    }

    /// <summary>
    /// Updates the specified model.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public Task<TModel> UpdateAsync(TModel model)
    {
        return Client.UpdateAsync(model);
    }

    /// <summary>
    /// Deletes the specified model.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public Task<TModel> DeleteAsync(TModel model)
    {
        return Client.DeleteAsync(model);
    }

    /// <inheritdoc />
    public Task<List<TModel>> FindByParentAsync(string parentId, string foreignKey)
    {
        return Client.FindByParentAsync(parentId,foreignKey);
    }

    /// <inheritdoc />
    public Task<List<TModel>> FindByParametersAsync(byte[] parameterBytes)
    {
        return Client.FindByParametersAsync(parameterBytes);
    }

    /// <inheritdoc />
    public Task CollectionChanged()
    {
        return Client.CollectionChanged();
    }

    /// <inheritdoc />
    public THub FireAndForget()
    {
        return Client.FireAndForget();
    }

    /// <inheritdoc />
    public Task DisposeAsync()
    {
        return Client.DisposeAsync();
    }

    /// <inheritdoc />
    public Task WaitForDisconnect()
    {
        return Client.WaitForDisconnect();
    }

    public Task KeepAliveAsync()
    {
        return Client.KeepAliveAsync();
    }
}