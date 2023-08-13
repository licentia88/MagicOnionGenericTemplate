using MagicOnion.Server;
using MagicT.Server.Extensions;
using MagicT.Server.Services;

namespace MagicT.Server.Filters;

public sealed class UserFilterAttribute : MagicOnionFilterAttribute
{
    public override async ValueTask Invoke(ServiceContext context, Func<ServiceContext, ValueTask> next)
    {
        if(context.MethodInfo.Name is  nameof(UserService.LoginAsync) or nameof(UserService.RegisterAsync))
            context.AddItem("public-bin");

        await next(context);
    }
}
