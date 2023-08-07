using MagicOnion.Client;
using MagicT.Client.Exceptions;
using MagicT.Client.Models;
using MagicT.Redis.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Client.Filters;

internal class RateLimiterFilter : IClientFilter
{
    private MagicTUserData MagicTUserData { get; }

    private RateLimiterService RateLimiterService { get; }

    private ClientBlockerService ClientBlockerService { get; }

    public RateLimiterFilter(IServiceProvider provider)
    {
        using var scope = provider.CreateScope();

        MagicTUserData = scope.ServiceProvider.GetRequiredService<MagicTUserData>();

        ClientBlockerService = provider.GetService<ClientBlockerService>();

        RateLimiterService = provider.GetService<RateLimiterService>();
    }
 
    public async ValueTask<ResponseContext> SendAsync(RequestContext context, Func<RequestContext, ValueTask<ResponseContext>> next)
    {
        //ClientBlocker.RemoveBlock(MagicTUserData.Ip);
        if (ClientBlockerService.IsSoftBlocked(MagicTUserData.Ip))
            throw new FilterException("You are temporarily Banned");

        if (ClientBlockerService.IsHardBlocked(MagicTUserData.Ip))
            throw new FilterException("You are permanently Banned");

        if (RateLimiterService.CheckRateLimit(MagicTUserData.Ip))
            return await next(context);

        ClientBlockerService.AddSoftBlock(MagicTUserData.Ip);

        throw new FilterException("Request limit overdue");
    }
}