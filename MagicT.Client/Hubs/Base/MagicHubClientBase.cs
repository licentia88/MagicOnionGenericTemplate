using Coravel;
using Coravel.Scheduling.Schedule.Interfaces;
using Grpc.Net.Client;
using MagicOnion.Client;
using MagicOnion.Serialization.MemoryPack;
using MagicT.Shared.Enums;
using MagicT.Shared.Hubs.Base;
using MessagePipe;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
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
    private IPublisher<Operation, TModel> ModelPublisher { get; }
    private IPublisher<Operation, List<TModel>> ListPublisher { get; }
    public List<TModel> Collection { get; set; }
    protected THub Client;
    private IConfiguration Configuration { get; }
    private IScheduler Scheduler { get; }

    protected MagicHubClientBase(IServiceProvider provider)
    {
        Configuration = provider.GetService<IConfiguration>();
        Collection = provider.GetService<List<TModel>>();
        ModelPublisher = provider.GetService<IPublisher<Operation, TModel>>();
        ListPublisher = provider.GetService<IPublisher<Operation, List<TModel>>>();
        Scheduler = provider.GetService<IScheduler>();
    }

    public virtual async Task<Guid> ConnectAsync()
    {
#if (SSL_CONFIG)
        string baseUrl = Configuration.GetValue<string>("API_BASE_URL:HTTPS");
#else
        string baseUrl = Configuration.GetValue<string>("API_BASE_URL:HTTP");
#endif

#if (SSL_CONFIG)
        var SslAuthOptions = CreateSslClientAuthOptions();
        var socketHandler = CreateHttpClientWithSocketsHandler(SslAuthOptions, Timeout.InfiniteTimeSpan, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(30));
        var channelOptions = CreateGrpcChannelOptions(socketHandler);
        var channel = GrpcChannel.ForAddress(baseUrl, channelOptions);
#else
        var channel = GrpcChannel.ForAddress(baseUrl, new GrpcChannelOptions
        {
            MaxReceiveMessageSize = null,
            MaxSendMessageSize = null
        });
#endif

        Client = await StreamingHubClient.ConnectAsync<THub, TReceiver>(
            channel, this as TReceiver, null, default, MemoryPackMagicOnionSerializerProvider.Instance);

        var connectionId = await Client.ConnectAsync();

        Scheduler.ScheduleAsync(async () => await KeepAliveAsync()).EveryMinute();

        return connectionId;
    }

    /// <summary>
    /// This method is to keep the connection open
    /// </summary>
    /// <param name="state"></param>
    private async void KeepAliveAsync(object state)
    {
        await KeepAliveAsync();
    }

    public void OnCreate(TModel model)
    {
        Collection.Add(model);
        ModelPublisher.Publish(Operation.Create, model);
    }

    void IMagicReceiver<TModel>.OnRead(List<TModel> collection)
    {
        Collection.AddRange(collection);
        ListPublisher.Publish(Operation.Read, Collection);
    }

    void IMagicReceiver<TModel>.OnStreamRead(List<TModel> collection)
    {
        Collection.AddRange(collection);
        ListPublisher.Publish(Operation.Stream, collection);
    }

    public void OnUpdate(TModel model)
    {
        var index = Collection.IndexByKey(model);
        Collection[index] = model;
        ModelPublisher.Publish(Operation.Update, model);
    }

    public void OnDelete(TModel model)
    {
        var index = Collection.IndexByKey(model);
        Collection.RemoveAt(index);
        ModelPublisher.Publish(Operation.Delete, model);
    }

    public void OnCollectionChanged(List<TModel> collection)
    {
        Collection.Clear();
        Collection.AddRange(collection);
        ListPublisher.Publish(Operation.Read, Collection);
    }

    public async Task StreamReadAsync(int batchSize)
    {
        await Client.StreamReadAsync(batchSize);
    }

    public async Task<TModel> CreateAsync(TModel model)
    {
        return await Client.CreateAsync(model);
    }

    public Task ReadAsync()
    {
        return Client.ReadAsync();
    }

    public Task<TModel> UpdateAsync(TModel model)
    {
        return Client.UpdateAsync(model);
    }

    public Task<TModel> DeleteAsync(TModel model)
    {
        return Client.DeleteAsync(model);
    }

    public Task<List<TModel>> FindByParentAsync(string parentId, string foreignKey)
    {
        return Client.FindByParentAsync(parentId, foreignKey);
    }

    public Task<List<TModel>> FindByParametersAsync(byte[] parameterBytes)
    {
        return Client.FindByParametersAsync(parameterBytes);
    }

    public Task CollectionChanged()
    {
        return Client.CollectionChanged();
    }

    public THub FireAndForget()
    {
        return Client.FireAndForget();
    }

    public Task DisposeAsync()
    {
        return Client.DisposeAsync();
    }

    public Task WaitForDisconnect()
    {
        return Client.WaitForDisconnect();
    }

    public Task KeepAliveAsync()
    {
        return Client.KeepAliveAsync();
    }
}