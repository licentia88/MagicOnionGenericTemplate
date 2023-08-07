using Grpc.Core;
using MagicOnion.Client;
using MagicT.Client.Exceptions;
using Majorsoft.Blazor.Extensions.BrowserStorage;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Client.Filters;

public class AuthenticationFilter : IClientFilter
{
    ILocalStorageService StorageService { get; set; }

    public AuthenticationFilter(IServiceProvider provider)
    {
        StorageService = provider.GetService<ILocalStorageService>();
    }

    public async ValueTask<ResponseContext> SendAsync(RequestContext context, Func<RequestContext, ValueTask<ResponseContext>> next)
    {
        var token = await StorageService.GetItemAsync<byte[]>("auth-token-bin");

        if (token is null)
            throw new AuthException(StatusCode.NotFound, "Security Token not found");


        var tokenMetaData = new Metadata.Entry("auth-token-bin", token);

        var header = context.CallOptions.Headers;

    
        var existing = header.FirstOrDefault((Metadata.Entry arg) => arg.Key == "auth-token-bin");

        header.Remove(existing);

        header.Add(new Metadata.Entry("auth-token-bin", token));

        return await next(context);
    }
}