using Blazored.LocalStorage;
using Grpc.Core;
using Grpc.Net.Client;
using MagicOnion;
using MagicOnion.Client;
using MagicOnion.Serialization.MemoryPack;
//using Majorsoft.Blazor.Extensions.BrowserStorage;
using Microsoft.Extensions.Configuration;
#if (GRPC_SSL)
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
#endif
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Client.Services.Base;

public abstract class MagicClientServiceBase<TService>: IService<TService> where TService : IService<TService>
{
    protected readonly TService Client;

    public IConfiguration Configuration { get; set; }

    public IServiceProvider Provider { get; set; }

    public ILocalStorageService LocalStorageService { get; set; }

    protected private IConfigurationSection DockerConfig { get; set; }


    public MagicClientServiceBase(IServiceProvider provider) : this(provider, default)
    {

    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MagicClientService{TService,TModel}" /> class.
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="filters"></param>
    protected MagicClientServiceBase(IServiceProvider provider, params IClientFilter[] filters)
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

    public virtual TService WithOptions(CallOptions option)
    {
        return Client.WithOptions(option);
    }

    public virtual TService WithHeaders(Metadata headers)
    {
        return Client.WithHeaders(headers);
    }

    public virtual TService WithDeadline(DateTime deadline)
    {
        return Client.WithDeadline(deadline);
    }

    public virtual TService WithCancellationToken(CancellationToken cancellationToken)
    {
        return Client.WithCancellationToken(cancellationToken);
    }

    public virtual TService WithHost(string host)
    {
        return Client.WithHost(host);
    }
}
