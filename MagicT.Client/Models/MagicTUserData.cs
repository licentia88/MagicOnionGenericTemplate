using Microsoft.AspNetCore.Http;

namespace MagicT.Client.Models;

public class MagicTUserData
{
    /// <summary>
    /// The IP for the current session
    /// </summary>
    public string Ip { get; set; }

    public MagicTUserData(IHttpContextAccessor httpContextAccessor)
    {
        Ip = httpContextAccessor.HttpContext.Connection?.RemoteIpAddress.ToString();

    }
}

