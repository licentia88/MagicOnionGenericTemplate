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
