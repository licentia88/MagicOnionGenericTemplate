using Grpc.Core;
using MagicOnion.Client;
using MagicT.Client.Extensions;
using Majorsoft.Blazor.Extensions.BrowserStorage;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Client.Filters;

/// <summary>
///  Filter for adding user information to gRPC client requests.
/// </summary>
public sealed  class UserFilter : IClientFilter
{
    /// <summary>
    /// Local storage service
    /// </summary>
    public ILocalStorageService LocalStorageService { get; set; }
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="provider"></param>
    public UserFilter(IServiceProvider provider)
    {
        LocalStorageService = provider.GetService<ILocalStorageService>();
    }
    /// <inheritdoc/>
    public async ValueTask<ResponseContext> SendAsync(RequestContext context, Func<RequestContext, ValueTask<ResponseContext>> next)
    {
        if (context.MethodPath != "") return await next(context);
        
        var publicKey = await LocalStorageService.GetItemAsync<byte[]>("public-bin");
        
        context.CallOptions.Headers.AddorUpdateItem("public-bin",publicKey);

        return await next(context);
    }
}
