using Microsoft.AspNetCore.Http;

namespace MagicT.Client.Models;


/// <summary>
///  Represents the data of a MagicT client.
/// </summary>
public sealed class MagicTClientData
{
    /// <summary>
    ///  The IP for the current session
    /// </summary>
    public string Ip  => HttpContextAccessor.HttpContext.Connection?.RemoteIpAddress.ToString();
 
    /// <summary>
    ///  The user agent for the current session
    /// </summary>
    internal IHttpContextAccessor HttpContextAccessor { get; set; }

 
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="httpContextAccessor"></param>
    public MagicTClientData(IHttpContextAccessor httpContextAccessor)
    {
        HttpContextAccessor = httpContextAccessor;
    }
    
    
}
 