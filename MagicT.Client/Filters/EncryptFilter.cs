using MagicOnion.Client;

namespace MagicT.Client.Filters;


public class EncryptFilter : IClientFilter
{
    public async ValueTask<ResponseContext> SendAsync(RequestContext context, Func<RequestContext, ValueTask<ResponseContext>> next)
    {
        //context.SetRequestMutator(bytes => Encrypt(bytes));
        //context.SetResponseMutator(bytes => Decrypt(bytes));

        return await next(context);
    }
}

