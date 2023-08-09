using MagicOnion.Client;

namespace MagicT.Client.Filters;

public class EncryptFilter : IClientFilter
{
    public async ValueTask<ResponseContext> SendAsync(RequestContext context, Func<RequestContext, ValueTask<ResponseContext>> next)
    {
        return await next(context);
    }
}