using MagicOnion.Server;
using MagicT.Server.Extensions;

namespace MagicT.Server.Filters;

public sealed class KeyExchangeFilterAttribute : MagicOnionFilterAttribute
{
    public override async ValueTask Invoke(ServiceContext context, Func<ServiceContext, ValueTask> next)
    {
        context.AddItem("public-bin");
      
        await next(context);
    }
}