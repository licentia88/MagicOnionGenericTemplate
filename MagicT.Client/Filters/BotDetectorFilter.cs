using System.Security.Authentication;
using DeviceDetectorNET.Parser;
using MagicOnion.Client;
using MagicT.Client.Models;
using MagicT.Redis.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System.Collections.Concurrent;

namespace MagicT.Client.Filters;

/// <summary>
/// Filter for detecting bots and blocking them.
/// </summary>
internal sealed class BotDetectorFilter : IClientFilter
{
    private MagicTClientData MagicTUserData { get; }
    private ClientBlockerService ClientBlockerService { get; }
    private static readonly ConcurrentDictionary<string, bool> BotCache = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="BotDetectorFilter"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    public BotDetectorFilter(IServiceProvider provider)
    {
        MagicTUserData = provider.GetService<MagicTClientData>();
        ClientBlockerService = provider.GetService<ClientBlockerService>();
    }

    /// <summary>
    /// Determines if the user agent is a bot.
    /// </summary>
    /// <param name="userAgent">The user agent string.</param>
    /// <returns><c>true</c> if the user agent is a bot; otherwise, <c>false</c>.</returns>
    private bool IsBot(StringValues userAgent)
    {
        if (BotCache.TryGetValue(userAgent, out var isBot))
        {
            return isBot;
        }

        var botParser = new BotParser { DiscardDetails = true };
        botParser.SetUserAgent(userAgent);
        var result = botParser.Parse();

        isBot = result.Match is not null;
        BotCache[userAgent] = isBot;

        return isBot;
    }

    /// <summary>
    /// Sends the request and blocks the client if it is detected as a bot.
    /// </summary>
    /// <param name="context">The request context.</param>
    /// <param name="next">The next step in the filter pipeline.</param>
    /// <returns>The response context.</returns>
    public async ValueTask<ResponseContext> SendAsync(RequestContext context, Func<RequestContext, ValueTask<ResponseContext>> next)
    {
        var httpContext = MagicTUserData.HttpContextAccessor.HttpContext;
        var agent = httpContext.Request.Headers["User-Agent"];

        if (!IsBot(agent))
        {
            return await next(context);
        }

        ClientBlockerService.AddSoftBlock(MagicTUserData.Ip);
        throw new AuthenticationException("You are temporarily banned");
    }
}