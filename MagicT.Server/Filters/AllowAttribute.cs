using MagicOnion.Server;

namespace MagicT.Server.Filters;

/// <summary>
/// A custom filter attribute for the MagicOnion framework.
/// This filter can be used to add custom logic to the server-side pipeline.
/// </summary>
public sealed class AllowAttribute : MagicOnionFilterAttribute
{
    /// <summary>
    /// Invokes the custom filter logic in the server-side pipeline.
    /// </summary>
    /// <param name="context">The ServiceContext representing the current request context.</param>
    /// <param name="next">The next filter or target method in the pipeline.</param>
    /// <returns>A task representing the asynchronous filter invocation.</returns>
    public override async ValueTask Invoke(ServiceContext context, Func<ServiceContext, ValueTask> next)
    {
        // Add your custom filter logic here, if needed.
        // This is a placeholder filter that simply calls the next filter or target method.
        // You can implement your specific filtering, validation, or authorization logic here.

        // Call the next filter or the target method in the pipeline.
        await next(context);
    }
}
