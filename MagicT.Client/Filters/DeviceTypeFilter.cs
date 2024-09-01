using MagicOnion.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System.Collections.Concurrent;
using MagicT.Client.Models;

namespace MagicT.Client.Filters;

/// <summary>
/// Filter for handling requests based on device type.
/// </summary>
internal sealed class DeviceTypeFilter : IClientFilter
{
    private MagicTClientData MagicTUserData { get; }

    private static readonly ConcurrentDictionary<string, string> DeviceTypeCache = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceTypeFilter"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    /// <summary>
    /// Determines the device type from the user agent.
    public DeviceTypeFilter(IServiceProvider provider)
    {
        MagicTUserData = provider.GetService<MagicTClientData>();
    }

    /// </summary>
    /// <param name="userAgent">The user agent string.</param>
    /// <returns>The device type.</returns>
    private string GetDeviceType(StringValues userAgent)
    {
        if (DeviceTypeCache.TryGetValue(userAgent, out var deviceType))
        {
            return deviceType;
        }

        var deviceDetector = new DeviceDetectorNET.DeviceDetector();
        deviceDetector.SetUserAgent(userAgent);
        deviceDetector.Parse();
        deviceDetector.GetDeviceName();
         
        DeviceTypeCache[userAgent] = deviceType;
        
        return deviceType;
    }

    /// <summary>
    /// Sends the request and handles it based on the device type.
    /// </summary>
    /// <param name="context">The request context.</param>
    /// <param name="next">The next step in the filter pipeline.</param>
    /// <returns>The response context.</returns>
    public async ValueTask<ResponseContext> SendAsync(RequestContext context, Func<RequestContext, ValueTask<ResponseContext>> next)
    {
        var httpContext = MagicTUserData.HttpContextAccessor.HttpContext;
        var agent = httpContext.Request.Headers["User-Agent"];
        var deviceType = GetDeviceType(agent);

        // Implement logic based on device type
        if (deviceType == "Mobile")
        {
            // Handle mobile-specific logic
        }

        return await next(context);
    }
}