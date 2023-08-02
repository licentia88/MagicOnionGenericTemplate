using MagicOnion;
using MagicOnion.Server;

namespace MagicT.Server.Filters;

public class AllowAttribute : MagicOnionFilterAttribute
{
    public override async ValueTask Invoke(ServiceContext context, Func<ServiceContext, ValueTask> next)
    {
        await next(context);
    }
}

 


