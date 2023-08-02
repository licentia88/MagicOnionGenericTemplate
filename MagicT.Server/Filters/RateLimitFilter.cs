using MagicOnion.Server;

namespace MagicT.Server.Filters;

// Define a rate limiting filter
public class RateLimiterAttribute : MagicOnionFilterAttribute
{
    private readonly int requestLimit;
    private readonly TimeSpan timeWindow;
    RateLimitHelper rateLimitHelper;
    public RateLimiterAttribute(int requestLimit, int timeWindow)
    {
        rateLimitHelper = new RateLimitHelper();
        this.requestLimit = requestLimit;
        this.timeWindow = TimeSpan.FromMinutes(timeWindow);
    }

    public override async ValueTask Invoke(ServiceContext context, Func<ServiceContext, ValueTask> next)
    {
 
        // Get client identifier, e.g., client IP address or user ID.
        Guid clientId = GetClientIdFromContext(context);
        //00-c61c4c13947efd03447233561236382e-9e5871c53cc8e8f1-00
        var result = rateLimitHelper.CheckRateLimit(clientId, requestLimit, timeWindow);



        // If rate limit is not exceeded, continue processing the request.
        await next(context);
    }

    private Guid GetClientIdFromContext(ServiceContext context)
    {
        // In a real-world scenario, you'd extract the client identifier (e.g., IP address, user ID) from the context.
        // For simplicity, we'll return a constant string "example_client_id" for demonstration purposes.
        return context.ContextId;
    }
 
}