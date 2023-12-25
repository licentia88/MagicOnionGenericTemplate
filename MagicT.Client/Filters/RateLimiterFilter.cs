using System.Security.Authentication;
using DeviceDetectorNET.Parser;
using MagicOnion.Client;
using MagicT.Client.Models;
using MagicT.Redis.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace MagicT.Client.Filters;

internal sealed class BotDetectorFilter : IClientFilter
{
    private MagicTClientData MagicTUserData { get; }

    private ClientBlockerService ClientBlockerService { get; }

    public BotDetectorFilter(IServiceProvider provider)
    {
        MagicTUserData = provider.GetService<MagicTClientData>();
        ClientBlockerService = provider.GetService<ClientBlockerService>();
    }

    private bool IsBot(StringValues userAgent)
    {
        var botParser = new BotParser();
        botParser.SetUserAgent(userAgent);

        // OPTIONAL: discard bot information. Parse() will then return true instead of information
        botParser.DiscardDetails = true;

        var result = botParser.Parse();

        return result.Match is not null;

    }
    public async ValueTask<ResponseContext> SendAsync(RequestContext context, Func<RequestContext, ValueTask<ResponseContext>> next)
    {
 
        var httpContext = MagicTUserData.HttpContextAccessor.HttpContext;

        var agent = httpContext.Request.Headers["User-Agent"];

        if (!IsBot(agent)) return await next(context);

        ClientBlockerService.AddSoftBlock(MagicTUserData.Ip);

        throw new AuthenticationException("You are temporarily banned");
 
    }
}

/// <summary>
/// Filter for rate limiting client requests.
/// </summary>
internal sealed class RateLimiterFilter : IClientFilter
{
    private MagicTClientData MagicTUserData { get; }
    private RateLimiterService RateLimiterService { get; }
    private ClientBlockerService ClientBlockerService { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RateLimiterFilter"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    public RateLimiterFilter(IServiceProvider provider)
    {
        using var scope = provider.CreateScope();

        MagicTUserData = scope.ServiceProvider.GetRequiredService<MagicTClientData>();
        ClientBlockerService = provider.GetService<ClientBlockerService>();
        RateLimiterService = provider.GetService<RateLimiterService>();
    }

    /// <summary>
    /// Checks and applies rate limiting logic before sending the request.
    /// </summary>
    /// <param name="context">The request context.</param>
    /// <param name="next">The next step in the filter pipeline.</param>
    /// <returns>The response context.</returns>
    public async ValueTask<ResponseContext> SendAsync(RequestContext context, Func<RequestContext, ValueTask<ResponseContext>> next)
    {
        if (ClientBlockerService.IsSoftBlocked(MagicTUserData.Ip))
            throw new AuthenticationException("You are temporarily banned");

        if (ClientBlockerService.IsHardBlocked(MagicTUserData.Ip))
            throw new AuthenticationException("You are permanently banned");

        if (RateLimiterService.CheckRateLimit(MagicTUserData.Ip))
            return await next(context);

        ClientBlockerService.AddSoftBlock(MagicTUserData.Ip);

        throw new AuthenticationException("Request limit exceeded");
    }
}
