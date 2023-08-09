using Grpc.Core;
using MagicOnion.Client;
using MagicT.Client.Exceptions;
using Majorsoft.Blazor.Extensions.BrowserStorage;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Client.Filters
{
    /// <summary>
    /// Filter for adding authentication token to gRPC client requests.
    /// </summary>
    public class AuthenticationFilter : IClientFilter
    {
        private ILocalStorageService StorageService { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationFilter"/> class.
        /// </summary>
        /// <param name="provider">The service provider.</param>
        public AuthenticationFilter(IServiceProvider provider)
        {
            StorageService = provider.GetService<ILocalStorageService>();
        }

        /// <summary>
        /// Adds the authentication token to the request headers and sends the request.
        /// </summary>
        /// <param name="context">The request context.</param>
        /// <param name="next">The next step in the filter pipeline.</param>
        /// <returns>The response context.</returns>
        public async ValueTask<ResponseContext> SendAsync(RequestContext context, Func<RequestContext, ValueTask<ResponseContext>> next)
        {
            var token = await StorageService.GetItemAsync<byte[]>("auth-token-bin");

            if (token is null)
                throw new AuthException(StatusCode.NotFound, "Security Token not found");

            var tokenMetaData = new Metadata.Entry("auth-token-bin", token);

            var header = context.CallOptions.Headers;

            var existing = header.FirstOrDefault((Metadata.Entry arg) => arg.Key == "auth-token-bin");
            header.Remove(existing);

            header.Add(new Metadata.Entry("auth-token-bin", token));

            return await next(context);
        }
    }
}
