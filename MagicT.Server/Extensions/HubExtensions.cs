using System.Linq;
using MagicOnion.Server;

namespace MagicT.Server.Extensions
{
    /// <summary>
    /// Provides extension methods for the MagicOnion framework, specifically targeting the ServiceContext type.
    /// </summary>
    public static class ServiceContextExtensions
    {
        /// <summary>
        /// Gets the client name from the request headers in the ServiceContext.
        /// </summary>
        /// <param name="context">The ServiceContext instance.</param>
        /// <returns>The client name extracted from the request headers.</returns>
        public static string GetClientName(this ServiceContext context)
        {
            return context.CallContext.RequestHeaders.FirstOrDefault(x => x.Key == "client")?.Value;
        }
    }
}
