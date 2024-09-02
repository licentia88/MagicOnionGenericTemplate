using MagicOnion.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System.Collections.Concurrent;
using DeviceDetectorNET;
using DeviceDetectorNET.Cache;
using DeviceDetectorNET.Parser.Device;
using MagicT.Client.Models;

namespace MagicT.Client.Filters;

    /// <summary>
    /// Filter for handling requests based on device type.
    /// </summary>
    // ReSharper disable once UnusedType.Global
    internal sealed class DeviceDetectionFilter : IClientFilter
    {
        private readonly MagicTClientData _magicTUserData;
        private static readonly ConcurrentDictionary<string, DeviceInfo> DeviceInfoCache = new();

        static DeviceDetectionFilter()
        {
            // Configure DeviceDetector settings
            DeviceDetectorSettings.LRUCacheMaxSize = 100000;
            DeviceDetectorSettings.LRUCacheCleanPercentage = 20;
            DeviceDetectorSettings.LRUCacheMaxDuration = TimeSpan.FromHours(1);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceDetectionFilter"/> class.
        /// </summary>
        /// <param name="provider">The service provider.</param>
        public DeviceDetectionFilter(IServiceProvider provider)
        {
            _magicTUserData = provider.GetRequiredService<MagicTClientData>();
        }

        /// <summary>
        /// Sends the request and handles it based on the device type.
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
                Console.WriteLine("Invalid User-Agent");
                return await next(context);
            }

            if (!DeviceInfoCache.TryGetValue(agent, out var deviceInfo))
            {
                var deviceDetector = LRUCachedDeviceDetector.GetDeviceDetector(agent);
                deviceInfo = GetDeviceInfo(deviceDetector);
                DeviceInfoCache.TryAdd(agent, deviceInfo);
            }

            Console.WriteLine($"This is a {deviceInfo.DeviceType}");
            // You can access other device information like this:
            // Console.WriteLine($"Brand: {deviceInfo.Brand}, Model: {deviceInfo.Model}, OS: {deviceInfo.OS}");

            return await next(context);
        }

        private DeviceInfo GetDeviceInfo(DeviceDetector deviceDetector)
        {
            string deviceType;
            if (deviceDetector.IsMobile())
                deviceType = "Smartphone";
            else if (deviceDetector.IsTablet())
                deviceType = "Tablet";
            else if (deviceDetector.IsDesktop())
                deviceType = "Desktop";
            else if (deviceDetector.IsTv())
                deviceType = "TV";
            else
                deviceType = "Unknown";

            return new DeviceInfo
            {
                DeviceType = deviceType,
                Brand = deviceDetector.GetBrandName(),
                Model = deviceDetector.GetModel(),
                OS = deviceDetector.GetOs().ToString()
            };
        }
        
        private class DeviceInfo
        {
            public string DeviceType { get; set; }
            public string Brand { get; set; }
            public string Model { get; set; }
            public string OS { get; set; }
        }
    }

    
