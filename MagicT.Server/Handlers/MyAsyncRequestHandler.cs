namespace MagicT.Server.Handlers;


/// <summary>
/// An experimental asynchronous request handler.
/// </summary>
[RegisterSingleton]
public class MyAsyncRequestHandler : MessagePipe.IAsyncRequestHandler<int, string>
{
    /// <summary>
    /// Handles the asynchronous request.
    /// </summary>
    /// <param name="request">The request parameter.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, containing the result string.</returns>
    public ValueTask<string> InvokeAsync(int request, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult("Do something here");
    }
}