using System.Net;

namespace MagicT.Client.Helpers;


/// <summary>
/// A handler for sending HTTP requests with specific version settings.
/// </summary>
public sealed class HttpVersionHandler : DelegatingHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HttpVersionHandler"/> class.
    /// </summary>
    public HttpVersionHandler() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpVersionHandler"/> class with a custom HTTP message handler.
    /// </summary>
    /// <param name="httpMessageHandler">The inner HTTP message handler.</param>
    public HttpVersionHandler(HttpMessageHandler httpMessageHandler) : base(httpMessageHandler) { }

    /// <summary>
    /// Sends an HTTP request with specified version settings.
    /// </summary>
    /// <param name="request">The HTTP request message to send.</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <returns>A task representing the asynchronous operation and holding the HTTP response message.</returns>
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Set the HTTP version to 1.1 and enforce exact version in the request
        request.Version = HttpVersion.Version11;

        // Set
        //request.Version = HttpVersion.Version20;

        request.VersionPolicy = HttpVersionPolicy.RequestVersionExact;

        return base.SendAsync(request, cancellationToken);
    }
}

