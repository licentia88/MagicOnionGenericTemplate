using MagicOnion.Server;
using MagicT.Server.Extensions;
using MagicT.Server.Services;

namespace MagicT.Server.Filters;

/// <summary>
/// Filter attribute to handle public key exchange during login/register.
/// Adds the received public key to the context for key agreement. 
/// </summary>
public sealed class KeyExchangeFilterAttribute : MagicOnionFilterAttribute
{
    /// <summary>
    /// Called before the service method execution.
    /// </summary>
    public override async ValueTask Invoke(ServiceContext context, Func<ServiceContext, ValueTask> next)
    {
        // Check if it's the login or register endpoint
        if (context.MethodInfo.Name is nameof(UserService.LoginWithPhoneAsync) or nameof(UserService.LoginWithEmailAsync) or nameof(UserService.RegisterAsync) )
            context.AddItem("public-bin");

        // Proceed to endpoint execution
        await next(context);
    }
}
