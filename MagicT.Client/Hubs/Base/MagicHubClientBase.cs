using Coravel.Scheduling.Schedule.Interfaces;
using Grpc.Net.Client;
using MagicOnion.Client;
using MagicOnion.Serialization.MemoryPack;
using MagicT.Shared.Enums;
using MagicT.Shared.Hubs.Base;
using MessagePipe;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using MagicT.Shared.Helpers;

namespace MagicT.Client.Hubs.Base
{
    /// <summary>
    /// Represents the base class for Magic Hub Clients, providing common functionality for gRPC hub communication.
    /// </summary>
    /// <typeparam name="THub">The type of the hub interface.</typeparam>
    /// <typeparam name="TReceiver">The type of the receiver interface.</typeparam>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    public abstract class MagicHubClientBase<THub, TReceiver, TModel> : IMagicReceiver<TModel>, IMagicHub<THub, TReceiver, TModel>
        where THub : IMagicHub<THub, TReceiver, TModel>
        where TReceiver : class, IMagicReceiver<TModel>
    {
        /// <summary>
        /// Publishes operations for individual models.
        /// </summary>
        private IPublisher<Operation, TModel> ModelPublisher { get; }

        /// <summary>
        /// Publishes operations for collections of models.
        /// </summary>
        private IPublisher<Operation, List<TModel>> ListPublisher { get; }

        /// <summary>
        /// Gets or sets the collection of models.
        /// </summary>
        public List<TModel> Collection { get; set; }

        /// <summary>
        /// The gRPC hub client instance.
        /// </summary>
        public THub Client { get; set; }

        /// <summary>
        /// Gets the configuration instance.
        /// </summary>
        private IConfiguration Configuration{ get; }

        /// <summary>
        /// Gets the scheduler instance.
        /// </summary>
        private IScheduler Scheduler { get; }

        /// <summary>
        /// Gets or sets a value indicating whether to use SSL for gRPC communication.
        /// </summary>
        protected bool UseSsl { get; }

        /// <summary>
        /// Gets or sets the base URL for the gRPC channel.
        /// </summary>
        public string BaseUrl { get; set; }
        
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MagicHubClientBase{THub, TReceiver, TModel}"/> class with the specified service provider.
        /// </summary>
        /// <param name="provider">The service provider.</param>
        protected MagicHubClientBase(IServiceProvider provider)
        {
            Configuration = provider.GetService<IConfiguration>();
            Collection = provider.GetService<List<TModel>>();
            ModelPublisher = provider.GetService<IPublisher<Operation, TModel>>();
            ListPublisher = provider.GetService<IPublisher<Operation, List<TModel>>>();
            Scheduler = provider.GetService<IScheduler>();
            BaseUrl = Configuration.GetValue<string>(UseSsl && PlatFormHelper.IsWindows() ? "API_BASE_URL:HTTPS" : "API_BASE_URL:HTTP");


#if (SSL_CONFIG)
    UseSsl = true;
#else
            UseSsl = false;
#endif
        }
        
        /// <summary>
        /// Connects to the gRPC hub asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous connect operation. The task result contains the connection ID.</returns>
        public virtual async Task<Guid> ConnectAsync()
        {
            var channel = CreateChannel(BaseUrl);
 
            Client = await StreamingHubClient.ConnectAsync<THub, TReceiver>(
                channel, this as TReceiver, null, default, MemoryPackMagicOnionSerializerProvider.Instance);

            var connectionId = await Client.ConnectAsync();

            Scheduler.ScheduleAsync(KeepAliveAsync).EveryMinute();

            return connectionId;
        }

        /// <summary>
        /// Creates a gRPC channel based on SSL configuration.
        /// </summary>
        /// <param name="baseUrl">The base URL for the gRPC channel.</param>
        /// <returns>A configured gRPC channel.</returns>
        private GrpcChannel CreateChannel(string baseUrl)
        {
            var channelOptions = new GrpcChannelOptions
            {
                MaxReceiveMessageSize = null,
                MaxSendMessageSize = null
            };

            if (!UseSsl || !PlatFormHelper.IsWindows()) 
                return GrpcChannel.ForAddress(baseUrl, channelOptions);
            
            var sslAuthOptions = ConfigureSslClientAuthOptions();
            var socketHandler = ConfigureHttpClientWithSocketsHandler(sslAuthOptions, Timeout.InfiniteTimeSpan, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(30));
            channelOptions.HttpHandler = socketHandler;

            return GrpcChannel.ForAddress(baseUrl, channelOptions);
        }

        /// <summary>
        /// Configures SSL client authentication options.
        /// </summary>
        /// <param name="certificate">The optional certificate for SSL authentication.</param>
        /// <returns>An instance of <see cref="SslClientAuthenticationOptions"/> configured with the provided certificate.</returns>
        private SslClientAuthenticationOptions ConfigureSslClientAuthOptions(X509Certificate2 certificate = null)
        {
            return new SslClientAuthenticationOptions
            {
                RemoteCertificateValidationCallback = (sender, cert, chain, errors) =>
                {
                    if (errors == SslPolicyErrors.None)
                        return true;

                    if (certificate == null)
                        return false;

                    var x509Chain = new X509Chain
                    {
                        ChainPolicy = { RevocationMode = X509RevocationMode.NoCheck }
                    };

                    return cert != null && x509Chain.Build(new X509Certificate2(cert));
                },
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
        private SocketsHttpHandler ConfigureHttpClientWithSocketsHandler(SslClientAuthenticationOptions sslClientAuthenticationOptions,
            TimeSpan pooledConnectionIdleTimeout,
            TimeSpan keepAlivePingDelay,
            TimeSpan keepAlivePingTimeout,
            bool enableMultipleHttp2Connections = true)
        {
            return new SocketsHttpHandler
            {
                SslOptions = sslClientAuthenticationOptions,
                PooledConnectionIdleTimeout = pooledConnectionIdleTimeout,
                KeepAlivePingDelay = keepAlivePingDelay,
                KeepAlivePingTimeout = keepAlivePingTimeout,
                EnableMultipleHttp2Connections = enableMultipleHttp2Connections
            };
        }

        /// <summary>
        /// Handles the creation of a model.
        /// </summary>
        /// <param name="model">The model to create.</param>
        public void OnCreate(TModel model)
        {
            Collection.Add(model);
            ModelPublisher.Publish(Operation.Create, model);
        }

        /// <summary>
        /// Handles the reading of a collection of models.
        /// </summary>
        /// <param name="collection">The collection of models to read.</param>
        void IMagicReceiver<TModel>.OnRead(List<TModel> collection)
        {
            Collection = collection;
            ListPublisher.Publish(Operation.Read, collection);
        }

        /// <summary>
        /// Handles the streaming read of a collection of models.
        /// </summary>
        /// <param name="collection">The collection of models to stream read.</param>
        void IMagicReceiver<TModel>.OnStreamRead(List<TModel> collection)
        {
            Collection.AddRange(collection);
            ListPublisher.Publish(Operation.Stream, collection);
        }

        /// <summary>
        /// Handles the update of a model.
        /// </summary>
        /// <param name="model">The model to update.</param>
        public void OnUpdate(TModel model)
        {
            var index = Collection.FindIndex(m => m.Equals(model));
            if (index >= 0)
            {
                Collection[index] = model;
                ModelPublisher.Publish(Operation.Update, model);
            }
        }

        /// <summary>
        /// Handles the deletion of a model.
        /// </summary>
        /// <param name="model">The model to delete.</param>
        public void OnDelete(TModel model)
        {
            Collection.Remove(model);
            ModelPublisher.Publish(Operation.Delete, model);
        }

        /// <summary>
        /// Handles the change of a collection of models.
        /// </summary>
        /// <param name="collection">The collection of models that changed.</param>
        public void OnCollectionChanged(List<TModel> collection)
        {
            Collection = collection;
            ListPublisher.Publish(Operation.Read, collection);
        }

        /// <summary>
        /// Streams read models asynchronously.
        /// </summary>
        /// <param name="batchSize">The batch size for streaming read.</param>
        /// <returns>A task that represents the asynchronous stream read operation.</returns>
        public async Task StreamReadAsync(int batchSize)
        {
            await Client.StreamReadAsync(batchSize);
        }

        /// <summary>
        /// Creates a model asynchronously.
        /// </summary>
        /// <param name="model">The model to create.</param>
        /// <returns>A task that represents the asynchronous create operation. The task result contains the created model.</returns>
        public async Task<TModel> CreateAsync(TModel model)
        {
            return await Client.CreateAsync(model);
        }

        /// <summary>
        /// Reads models asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        public Task ReadAsync()
        {
            return Client.ReadAsync();
        }

        /// <summary>
        /// Updates a model asynchronously.
        /// </summary>
        /// <param name="model">The model to update.</param>
        /// <returns>A task that represents the asynchronous update operation. The task result contains the updated model.</returns>
        public Task<TModel> UpdateAsync(TModel model)
        {
            return Client.UpdateAsync(model);
        }

        /// <summary>
        /// Deletes a model asynchronously.
        /// </summary>
        /// <param name="model">The model to delete.</param>
        /// <returns>A task that represents the asynchronous delete operation. The task result contains the deleted model.</returns>
        public Task<TModel> DeleteAsync(TModel model)
        {
            return Client.DeleteAsync(model);
        }

        /// <summary>
        /// Finds models by parent asynchronously.
        /// </summary>
        /// <param name="parentId">The parent ID.</param>
        /// <param name="foreignKey">The foreign key.</param>
        /// <returns>A task that represents the asynchronous find operation. The task result contains the list of found models.</returns>
        public Task<List<TModel>> FindByParentAsync(string parentId, string foreignKey)
        {
            return Client.FindByParentAsync(parentId, foreignKey);
        }

        /// <summary>
        /// Finds models by parameters asynchronously.
        /// </summary>
        /// <param name="parameterBytes">The parameters as bytes.</param>
        /// <returns>A task that represents the asynchronous find operation. The task result contains the list of found models.</returns>
        public Task<List<TModel>> FindByParametersAsync(byte[] parameterBytes)
        {
            return Client.FindByParametersAsync(parameterBytes);
        }

        /// <summary>
        /// Notifies that the collection has changed.
        /// </summary>
        /// <returns>A task that represents the asynchronous notification operation.</returns>
        public Task CollectionChanged()
        {
            return Client.CollectionChanged();
        }

        /// <summary>
        /// Returns a fire-and-forget hub client.
        /// </summary>
        /// <returns>A fire-and-forget hub client.</returns>
        public THub FireAndForget()
        {
            return Client.FireAndForget();
        }

        /// <summary>
        /// Disposes the hub client asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous dispose operation.</returns>
        public Task DisposeAsync()
        {
            return Client.DisposeAsync();
        }

        /// <summary>
        /// Waits for the hub client to disconnect.
        /// </summary>
        /// <returns>A task that represents the asynchronous wait operation.</returns>
        public Task WaitForDisconnect()
        {
            return Client.WaitForDisconnect();
        }

        /// <summary>
        /// Keeps the hub client connection alive.
        /// </summary>
        /// <returns>A task that represents the asynchronous keep-alive operation.</returns>
        public Task KeepAliveAsync()
        {
            return Client.KeepAliveAsync();
        }
    }
}