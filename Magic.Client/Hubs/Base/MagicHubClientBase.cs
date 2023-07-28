using System.Reflection;
using Grpc.Core;
using Grpc.Net.Client;
using Magic.Shared.Enums;
using Magic.Shared.Hubs.Base;
using Magic.Shared.Models.ServiceModels;
using MagicOnion.Client;
using MagicOnion.Serialization.MemoryPack;
using MessagePipe;
using Microsoft.Extensions.DependencyInjection;

namespace Magic.Client.Hubs.Base;

public abstract class MagicHubClientBase<THub, TReceiver, TModel> : IMagicReceiver<TModel>
    where THub : IMagicHub<THub, TReceiver,TModel> 
    where TReceiver : class, IMagicReceiver<TModel>
{
    protected THub Client;

    private IPublisher<Operation,TModel> ModelPublisher { get; set; }

    private IPublisher<Operation,List<TModel>> ListPublisher { get; set; }

    public List<TModel> Collection { get; set; }

    public MagicHubClientBase(IServiceProvider provider)
    {
        Collection = provider.GetService<List<TModel>>();
        ModelPublisher = provider.GetService<IPublisher<Operation,TModel>>();
        ListPublisher = provider.GetService<IPublisher<Operation,List<TModel>>>();
    }



    public virtual async Task ConnectAsync()
    {
        var channel = GrpcChannel.ForAddress("http://localhost:5002");

        Client = await StreamingHubClient.ConnectAsync<THub, TReceiver>(channel, this as TReceiver, null, SenderOption, MemoryPackMagicOnionSerializerProvider.Instance);

        await Client.ConnectAsync();
     }

    void IMagicReceiver<TModel>.OnCreate(TModel model)
    {
        Collection.Add(model);

        ModelPublisher.Publish(Operation.Create, model);
    }

    void IMagicReceiver<TModel>.OnRead(List<TModel> collection)
    {
        Collection.AddRange(collection);

        ListPublisher.Publish(Operation.Read,Collection);
    }

    void IMagicReceiver<TModel>.OnStreamRead(List<TModel> collection)
    {
        Collection.AddRange(collection);

        ListPublisher.Publish(Operation.Stream,collection);
    }


    void IMagicReceiver<TModel>.OnUpdate(TModel model)
    {
        var index = Collection.IndexOf(model);

        Collection[index] = model;

        ModelPublisher.Publish(Operation.Update, model);
    }

    void IMagicReceiver<TModel>.OnDelete(TModel model)
    {
        Collection.Remove(model);

        ModelPublisher.Publish(Operation.Delete, model);
    }

    void IMagicReceiver<TModel>.OnCollectionChanged(List<TModel> collection)
    {
        Collection.Clear();
        Collection.AddRange(collection);

        ListPublisher.Publish(Operation.Read, Collection);
        
    }


    public async Task StreamReadAsync()
    {
         await Client.StreamReadAsync(1);
    }


    public async Task<RESPONSE_RESULT<TModel>> CreateAsync(TModel model)
    {
        return await Client.CreateAsync(model);
    }

    public Task<RESPONSE_RESULT<List<TModel>>> ReadAsync()
    {
        return Client.ReadAsync();
    }

    public Task<RESPONSE_RESULT<TModel>> UpdateAsync(TModel model)
    {
        return Client.UpdateAsync(model);
    }

    public Task<RESPONSE_RESULT<TModel>> DeleteAsync(TModel model)
    {
        return Client.DeleteAsync(model);
    }

    private CallOptions SenderOption => new CallOptions().WithHeaders(new Metadata
    {
        { "client", Assembly.GetEntryAssembly()!.GetName().Name}
    });

}
