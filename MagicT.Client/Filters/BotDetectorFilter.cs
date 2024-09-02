using System.Security.Authentication;
using DeviceDetectorNET;
using DeviceDetectorNET.Cache;
using MagicOnion.Client;
using MagicT.Client.Models;
using MagicT.Redis.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace MagicT.Client.Filters;

/// <summary>
/// Filter for detecting bots and blocking them.
/// </summary>
// ReSharper disable once UnusedType.Global
internal sealed class BotDetectorFilter : IClientFilter
{
    private readonly MagicTClientData _magicTUserData;
    private readonly ClientBlockerService _clientBlockerService;
    private static readonly ConcurrentDictionary<string, bool> BotCache = new();

    static BotDetectorFilter()
    {
        // Configure DeviceDetector settings
        DeviceDetectorSettings.LRUCacheMaxSize = 100000; // Set maximum cache size
        DeviceDetectorSettings.LRUCacheCleanPercentage = 20; // Set clean percentage
        DeviceDetectorSettings.LRUCacheMaxDuration = TimeSpan.FromHours(1); // Set maximum duration
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BotDetectorFilter"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    public BotDetectorFilter(IServiceProvider provider)
    {
        _magicTUserData = provider.GetRequiredService<MagicTClientData>();
        _clientBlockerService = provider.GetRequiredService<ClientBlockerService>();
    }

    /// <summary>
    /// Sends the request and blocks the client if it is detected as a bot.
    /// </summary>
    /// <param name="context">The request context.</param>
    /// <param name="next">The next step in the filter pipeline.</param>
    /// <returns>The response context.</returns>
    public async ValueTask<ResponseContext> SendAsync(RequestContext context, Func<RequestContext, ValueTask<ResponseContext>> next)
    {
        var httpContext = _magicTUserData.HttpContextAccessor.HttpContext;
        var agent = httpContext.Request.Headers["User-Agent"].ToString();

        if (string.IsNullOrEmpty(agent))
        {
            _clientBlockerService.AddSoftBlock(_magicTUserData.Ip);
            throw new AuthenticationException("Invalid User-Agent");
        }

        if (!BotCache.TryGetValue(agent, out bool isBot))
        {
            var deviceDetector = LRUCachedDeviceDetector.GetDeviceDetector(agent);
            isBot = deviceDetector.IsBot();
            BotCache.TryAdd(agent, isBot);
        }

        if (!isBot) 
            return await next(context);
        
        _clientBlockerService.AddSoftBlock(_magicTUserData.Ip);
        throw new AuthenticationException("You are temporarily banned");

    }
}