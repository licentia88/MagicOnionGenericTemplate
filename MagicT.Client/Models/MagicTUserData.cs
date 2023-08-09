using Microsoft.AspNetCore.Http;

namespace MagicT.Client.Models;

public class MagicTClientData
{
    /// <summary>
    ///     The IP for the current session
    /// </summary>
    public string Ip { get; }
 
    public MagicTClientData(IHttpContextAccessor httpContextAccessor)
    {
        // var test = httpContextAccessor.HttpContext.User.Identity?.Name;
        Ip = httpContextAccessor.HttpContext.Connection?.RemoteIpAddress.ToString();
    }
}

 