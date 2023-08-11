﻿using System.Reflection;
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

public abstract class MagicHubClientBase<THub, TReceiver, TModel> : IMagicTReceiver<TModel>
    where THub : IMagicTHub<THub, TReceiver, TModel>
    where TReceiver : class, IMagicTReceiver<TModel>
{
    protected THub Client;

    public MagicHubClientBase(IServiceProvider provider)
    {
        Collection = provider.GetService<List<TModel>>();
        ModelPublisher = provider.GetService<IPublisher<Operation, TModel>>();
        ListPublisher = provider.GetService<IPublisher<Operation, List<TModel>>>();
    }

    private IPublisher<Operation, TModel> ModelPublisher { get; }

    private IPublisher<Operation, List<TModel>> ListPublisher { get; }

    public List<TModel> Collection { get; set; }

    private CallOptions SenderOption => new CallOptions().WithHeaders(new Metadata
    {
        {"client", Assembly.GetEntryAssembly()!.GetName().Name}
    });

    void IMagicTReceiver<TModel>.OnCreate(TModel model)
    {
        Collection.Add(model);

        ModelPublisher.Publish(Operation.Create, model);
    }

    void IMagicTReceiver<TModel>.OnRead(List<TModel> collection)
    {
        Collection.AddRange(collection);

        ListPublisher.Publish(Operation.Read, Collection);
    }

    void IMagicTReceiver<TModel>.OnStreamRead(List<TModel> collection)
    {
        Collection.AddRange(collection);

        ListPublisher.Publish(Operation.Stream, collection);
    }


    void IMagicTReceiver<TModel>.OnUpdate(TModel model)
    {
        var index = Collection.IndexOf(model);

        Collection[index] = model;

        ModelPublisher.Publish(Operation.Update, model);
    }

    void IMagicTReceiver<TModel>.OnDelete(TModel model)
    {
        Collection.Remove(model);

        ModelPublisher.Publish(Operation.Delete, model);
    }

    void IMagicTReceiver<TModel>.OnCollectionChanged(List<TModel> collection)
    {
        Collection.Clear();
        Collection.AddRange(collection);

        ListPublisher.Publish(Operation.Read, Collection);
    }


    /// <summary>
    /// Connects to the hub asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual async Task ConnectAsync()
    {
        var channel = GrpcChannel.ForAddress("http://localhost:5002");

        Client = await StreamingHubClient.ConnectAsync<THub, TReceiver>(
            channel, this as TReceiver, null, SenderOption, MemoryPackMagicOnionSerializerProvider.Instance);

        await Client.ConnectAsync();
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
}