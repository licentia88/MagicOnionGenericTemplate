using MagicOnion.Client;

namespace MagicT.Client.Filters;

/// <summary>
/// Filter for retrying client requests.
/// </summary>
internal sealed class RetryFilter : IClientFilter
{
    private readonly int _maxRetryAttempts;
    private readonly TimeSpan _retryDelay;

    /// <summary>
    /// Initializes a new instance of the <see cref="RetryFilter"/> class.
    /// </summary>
    /// <param name="maxRetryAttempts">The maximum number of retry attempts.</param>
    /// <param name="retryDelay">The delay between retries.</param>
    public RetryFilter(int maxRetryAttempts, TimeSpan retryDelay)
    {
        _maxRetryAttempts = maxRetryAttempts;
        _retryDelay = retryDelay;
    }

    /// <summary>
    /// Sends the request and retries if it fails.
    /// </summary>
    /// <param name="context">The request context.</param>
    /// <param name="next">The next step in the filter pipeline.</param>
    /// <returns>The response context.</returns>
    public async ValueTask<ResponseContext> SendAsync(RequestContext context, Func<RequestContext, ValueTask<ResponseContext>> next)
    {
        var attempt = 0;
        while (true)
        {
            try
            {
                return await next(context);
            }
            catch (Exception ex) when (attempt < _maxRetryAttempts)
            {
                attempt++;
                Console.WriteLine($"Attempt {attempt} failed: {ex.Message}. Retrying in {_retryDelay.TotalSeconds} seconds...");
                await Task.Delay(_retryDelay);
            }
        }
    }
}