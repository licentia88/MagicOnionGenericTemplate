using Grpc.Core;
using MagicOnion.Client;
using MagicT.Client.Exceptions;
using MagicT.Client.Models;
using MagicT.Redis;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Client.Filters;

public class RateLimiterFilter : IClientFilter
{
 
    public MagicTUserData MagicTUserData { get; set; }

    public RateLimiter RateLimiter { get; set; }

    public RateLimiterFilter(IServiceProvider provider )
    {
        using var scope = provider.CreateScope();
        MagicTUserData = scope.ServiceProvider.GetRequiredService<MagicTUserData>();

        RateLimiter = provider.GetService<RateLimiter>();

    }
    public async ValueTask<ResponseContext> SendAsync(RequestContext context, Func<RequestContext, ValueTask<ResponseContext>> next)
    {
        if (RateLimiter.CheckRateLimit(MagicTUserData.Ip))
            return await next(context);

        throw new FilterException("Request limit overdue", StatusCode.PermissionDenied);
    }
}

