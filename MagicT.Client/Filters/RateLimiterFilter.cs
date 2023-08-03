using MagicOnion.Client;
using MagicT.Client.Exceptions;
using MagicT.Client.Models;
using MagicT.Redis;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Client.Filters;

public class RateLimiterFilter : IClientFilter
{
    private MagicTUserData MagicTUserData { get; }

    private RateLimiter RateLimiter { get; }

    ClientBlocker ClientBlocker { get; }

    public RateLimiterFilter(IServiceProvider provider )
    {
        using var scope = provider.CreateScope();
        MagicTUserData = scope.ServiceProvider.GetRequiredService<MagicTUserData>();

        ClientBlocker = provider.GetService<ClientBlocker>();

        RateLimiter = provider.GetService<RateLimiter>();

    }

    public async ValueTask<ResponseContext> SendAsync(RequestContext context, Func<RequestContext, ValueTask<ResponseContext>> next)
    {
        //ClientBlocker.RemoveBlock(MagicTUserData.Ip);
        if (ClientBlocker.IsSoftBlocked(MagicTUserData.Ip))
            throw new FilterException("You are temporarily Banned");

        if (ClientBlocker.IsHardBlocked(MagicTUserData.Ip))
            throw new FilterException("You are permanently Banned");

        if (RateLimiter.CheckRateLimit(MagicTUserData.Ip))
            return await next(context);

        ClientBlocker.AddSoftBlock(MagicTUserData.Ip);

        throw new FilterException("Request limit overdue");
    }
}

