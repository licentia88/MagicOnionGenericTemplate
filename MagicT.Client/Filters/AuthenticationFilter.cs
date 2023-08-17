using Grpc.Core;
using MagicOnion.Client;
using MagicT.Client.Exceptions;
using MagicT.Client.Extensions;
using MagicT.Shared.Enums;
using Majorsoft.Blazor.Extensions.BrowserStorage;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Client.Filters;

/// <summary>
/// Filter for adding authentication token to gRPC client requests.
/// </summary>
public sealed class AuthenticationFilter : IClientFilter
{
    private ILocalStorageService StorageService { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationFilter"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    public AuthenticationFilter(IServiceProvider provider)
    {
        StorageService = provider.GetService<ILocalStorageService>();
    }

    /// <summary>
    /// Adds the authentication token to the request headers and sends the request.
    /// </summary>
    /// <param name="context">The request context.</param>
    /// <param name="next">The next step in the filter pipeline.</param>
    /// <returns>The response context.</returns>
    public async ValueTask<ResponseContext> SendAsync(RequestContext context, Func<RequestContext, ValueTask<ResponseContext>> next)
    {
        var token = await StorageService.GetItemAsync<byte[]>("token-bin");

        if (token is null)
            throw new AuthException(StatusCode.NotFound, "Security Token not found");
        
        context.CallOptions.Headers.AddorUpdateItem("token-bin", token);
      

        return await next(context);
    }
}