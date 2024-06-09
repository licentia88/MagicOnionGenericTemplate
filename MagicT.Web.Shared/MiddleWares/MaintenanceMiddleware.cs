using MagicT.Web.Shared.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace MagicT.Web.Shared.MiddleWares;

public class MaintenanceMiddleware
{
    private readonly RequestDelegate _next;

    MaintenanceModeOptions MaintenanceModeOptions;
    public MaintenanceMiddleware(RequestDelegate next, IOptions<MaintenanceModeOptions> options)
    {
        MaintenanceModeOptions = options.Value;
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        // Check if maintenance mode is enabled
        if (IsMaintenanceModeEnabled())
        {
            await context.Response.WriteAsync("Maintenance mode is enabled. Please try again later.");
            return;
        }

        await _next(context);
    }

    private bool IsMaintenanceModeEnabled()
    {
        return MaintenanceModeOptions.IsEnabled;
    }
}

