using MagicOnion.Server;

namespace MagicT.Server.Extensions;
public static class HubExtensions
{

    public static string GetClientName(this ServiceContext context)
    {
        return context.CallContext.RequestHeaders.FirstOrDefault(x => x.Key == "client").Value;
    }
}