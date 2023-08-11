using Grpc.Core;
using MagicOnion;
using MagicOnion.Server;
using MagicOnion.Server.Filters;
using MagicT.Server.Extensions;
using MagicT.Server.Jwt;

namespace MagicT.Server.Filters;

/// <summary>
/// Custom authorization filter for the MagicOnion framework to validate user roles based on JWT token.
/// </summary>
public sealed class MagicTAuthorize : Attribute, IMagicOnionFilterFactory<IMagicOnionServiceFilter>, IMagicOnionServiceFilter
{
    private int[] Roles { get; }

    private IServiceProvider ServiceProvider { get; set; }

    public MagicTTokenService MagicTTokenService { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MagicTAuthorize"/> class with the specified roles.
    /// </summary>
    /// <param name="roles">The required roles to access the service methods or hubs.</param>
    public MagicTAuthorize(params int[] roles)
    {
        this.Roles = roles;
    }

    /// <summary>
    /// Creates a new instance of the MagicTAuthorize filter using the provided service provider.
    /// </summary>
    /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
    /// <returns>The created MagicTAuthorize filter instance.</returns>
    public IMagicOnionServiceFilter CreateInstance(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;

        MagicTTokenService = ServiceProvider.GetRequiredService<MagicTTokenService>();

        return this;
    }

    /// <summary>
    /// Invokes the MagicTAuthorize filter logic in the server-side pipeline.
    /// </summary>
    /// <param name="context">The ServiceContext representing the current request context.</param>
    /// <param name="next">The next filter or target method in the pipeline.</param>
    /// <returns>A task representing the asynchronous filter invocation.</returns>
    public async ValueTask Invoke(ServiceContext context, Func<ServiceContext, ValueTask> next)
    {
        var isAllowed = context.AttributeLookup.Any(arg => arg.Key == typeof(AllowAttribute));

        if (!isAllowed)
        {
            var token = ProcessToken(context);

            ValidateRoles(token, Roles);
        }

        await next(context);
    }

    /// <summary>
    /// Processes the JWT token from the request headers.
    /// </summary>
    /// <param name="context">The ServiceContext representing the current request context.</param>
    /// <returns>The decoded MagicTToken from the JWT token.</returns>
    private MagicTToken ProcessToken(ServiceContext context)
    {
        var token =context.GetItemAs<byte[]>("token-bin");
        
        if (token is null)
            throw new ReturnStatusException(StatusCode.NotFound, "Security Token not found");

        return MagicTTokenService.DecodeToken(token);
    }

    /// <summary>
    /// Validates whether the user roles from the JWT token match the required roles.
    /// </summary>
    /// <param name="token">The MagicTToken representing the user's JWT token.</param>
    /// <param name="requiredRoles">The required roles to access the service methods or hubs.</param>
    private void ValidateRoles(MagicTToken token, params int[] requiredRoles)
    {
        if (!token.Roles.Any(requiredRoles.Contains))
            throw new ReturnStatusException(StatusCode.Unauthenticated, nameof(StatusCode.Unauthenticated));
    }
}