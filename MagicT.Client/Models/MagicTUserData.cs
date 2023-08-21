using Microsoft.AspNetCore.Http;

namespace MagicT.Client.Models;

/// <summary>
///  Represents the data of a MagicT client.
/// </summary>
public sealed class MagicTClientData
{
    /// <summary>
    ///     The IP for the current session
    /// </summary>
    public string Ip { get; }
 
    /// <summary>
    /// Initializes a new instance of the <see cref="MagicTClientData"/> class.
    /// </summary>
    /// <param name="httpContextAccessor"></param>
    public MagicTClientData(IHttpContextAccessor httpContextAccessor)
    {
        // var test = httpContextAccessor.HttpContext.User.Identity?.Name;
        Ip = httpContextAccessor.HttpContext.Connection?.RemoteIpAddress.ToString();
    }
}
 