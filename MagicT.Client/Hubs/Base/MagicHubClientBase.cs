using System.Reflection;
using Grpc.Core;
using Grpc.Net.Client;
using MagicOnion.Client;
using MagicOnion.Serialization.MemoryPack;
using MagicT.Shared.Enums;
using MagicT.Shared.Hubs.Base;
using MagicT.Shared.Models.ServiceModels;
using MessagePipe;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Client.Hubs.Base;

/// <summary>
/// Abstract base class for a generic service client implementation.
/// </summary>
/// <typeparam name="THub">The hub interface for the service.</typeparam>  
/// <typeparam name="TReceiver">The receiver interface for the client.</typeparam>
/// <typeparam name="TModel">The model type for the service.</typeparam>
public abstract partial class MagicHubClientBase<THub, TReceiver, TModel> : IMagicTReceiver<TModel>
    where THub : IMagicTHub<THub, TReceiver, TModel>
    where TReceiver : class, IMagicTReceiver<TModel>
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
    /// Creates a new instance of the client.
    /// </summary>
    /// <param name="provider">The dependency injection provider.</param>
    public MagicHubClientBase(IServiceProvider provider)
    {
        Collection = provider.GetService<List<TModel>>();
        ModelPublisher = provider.GetService<IPublisher<Operation, TModel>>();
        ListPublisher = provider.GetService<IPublisher<Operation, List<TModel>>>();
    }

    /// <summary>
    /// Connects to the service hub.
    /// </summary>
    /// <returns>A task representing the async operation.</returns>
    public virtual async Task ConnectAsync()
    {
#if (GRPC_SSL)
        var configuration = provider.GetService<IConfiguration>();
        //Make sure certificate file's copytooutputdirectory is set to always copy
        var certificatePath = Path.Combine(Environment.CurrentDirectory, configuration.GetSection("Certificate").Value);
        
        var certificate = new X509Certificate2(File.ReadAllBytes(certificatePath));

        var SslAuthOptions = CreateSslClientAuthOptions(certificate);

        var socketHandler = CreateHttpClientWithSocketsHandler(SslAuthOptions, Timeout.InfiniteTimeSpan, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(30));

        var channelOptions = CreateGrpcChannelOptions(socketHandler);

        var channel = GrpcChannel.ForAddress("https://localhost:7197", channelOptions);
#else
        var channel = GrpcChannel.ForAddress("http://localhost:5029"); 
#endif
        
        Client = await StreamingHubClient.ConnectAsync<THub, TReceiver>(
            channel, this as TReceiver, null, SenderOption, MemoryPackMagicOnionSerializerProvider.Instance);

        await Client.ConnectAsync();
    }
    

    private CallOptions SenderOption => new CallOptions().WithHeaders(new Metadata
    {
        {"client", Assembly.GetEntryAssembly()!.GetName().Name}
    });

    /// <summary>
    /// Called when a new model is created on the service side.
    /// </summary>
    /// <param name="model">The new model.</param>
    void IMagicTReceiver<TModel>.OnCreate(TModel model)
    {
        Collection.Add(model);

        ModelPublisher.Publish(Operation.Create, model);
    }

    /// <summary>
    /// Called when a full model collection is read from the service.
    /// </summary>
    /// <param name="collection">The full collection from the service.</param> 
    void IMagicTReceiver<TModel>.OnRead(List<TModel> collection)
    {
        Collection.AddRange(collection);

        ListPublisher.Publish(Operation.Read, Collection);
    }

    /// <summary>
    /// Called when a batch of models is streamed from the service.
    /// </summary>
    /// <param name="collection">The batch of models streamed.</param>
    void IMagicTReceiver<TModel>.OnStreamRead(List<TModel> collection)
    {
        Collection.AddRange(collection);

        ListPublisher.Publish(Operation.Stream, collection);
    }

    /// <summary>
    /// Called when a model is updated on the service side. 
    /// </summary>
    /// <param name="model">The updated model.</param>
    void IMagicTReceiver<TModel>.OnUpdate(TModel model)
    {
        var index = Collection.IndexOf(model);

        Collection[index] = model;

        ModelPublisher.Publish(Operation.Update, model);
    }

    /// <summary>
    /// Called when a model is deleted on the service side.
    /// </summary>
    /// <param name="model">The model that was deleted.</param>
    void IMagicTReceiver<TModel>.OnDelete(TModel model)
    {
        Collection.Remove(model);

        ModelPublisher.Publish(Operation.Delete, model);
    }

    /// <summary>
    /// Called when the entire model collection has changed on the service side. 
    /// </summary>
    /// <param name="collection">The updated collection from the service.</param>
    void IMagicTReceiver<TModel>.OnCollectionChanged(List<TModel> collection)
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
    public async Task<RESPONSE_RESULT<TModel>> CreateAsync(TModel model)
    {
        return await Client.CreateAsync(model);
    }

    
    /// <summary>
    /// Requests the service to retrieve all models.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the response from the service, including the model list if successful.
    /// </returns>
    public Task<RESPONSE_RESULT<List<TModel>>> ReadAsync()
    {
        return Client.ReadAsync();
    }

    /// <summary>
    /// Updates the specified model.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public Task<RESPONSE_RESULT<TModel>> UpdateAsync(TModel model)
    {
        return Client.UpdateAsync(model);
    }

    /// <summary>
    /// Deletes the specified model.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public Task<RESPONSE_RESULT<TModel>> DeleteAsync(TModel model)
    {
        return Client.DeleteAsync(model);
    }
}