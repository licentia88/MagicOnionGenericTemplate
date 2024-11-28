using Benutomo;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Shared.Managers;

/// <summary>
/// Manages cancellation tokens and provides methods to execute actions with cancellation support.
/// </summary>
[RegisterScoped]
[AutomaticDisposeImpl]
public partial class CancellationTokenManager : IDisposable,IAsyncDisposable
{
    [Inject] 
    [EnableAutomaticDispose] 
    public CancellationTokenSource CancellationTokenSource { get; set; }



    private readonly int _defaultTimeOut;

    /// <summary>
    /// Initializes a new instance of the <see cref="CancellationTokenManager"/> class.
    /// </summary>
    /// <param name="provider">Service Provider</param>
    /// <param name="configuration">The configuration to retrieve the default timeout value.</param>
    public CancellationTokenManager(IServiceProvider provider, IConfiguration configuration)
    {
        _defaultTimeOut = configuration.GetValue<int>("CancellationTokenTimeOut");
        CancellationTokenSource = provider.GetService<CancellationTokenSource>();
    }

    ~CancellationTokenManager()
    {
        Dispose(false);
    }
    /// <summary>
    /// Creates a cancellation token with the default timeout.
    /// </summary>
    /// <returns>A cancellation token.</returns>
    public CancellationToken CreateToken()
    {
        return CreateToken(_defaultTimeOut);
    }
    

    /// <summary>
    /// Creates a cancellation token with the specified timeout.
    /// </summary>
    /// <param name="timeoutMilliseconds">The timeout in milliseconds.</param>
    /// <returns>A cancellation token.</returns>
    public CancellationToken CreateToken(int timeoutMilliseconds)
    {
        CancellationTokenSource = new CancellationTokenSource(timeoutMilliseconds);
        return CancellationTokenSource.Token;
    }

    /// <summary>
    /// Cancels the current cancellation token.
    /// </summary>
    public void CancelToken()
    {
        CancellationTokenSource?.Cancel();
    }

    /// <summary>
    /// Executes an action with cancellation support.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains true if the task completed successfully; otherwise, false.</returns>
    public async Task<bool> ExecuteWithCancellationAsync(Action action, CancellationToken cancellationToken)
    {
        try
        {
            await Task.Run(action, cancellationToken);
            return true; // Task completed successfully
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Task was canceled.");
            return false; // Task was canceled
        }
    }

    /// <summary>
    /// Executes an action with a specified timeout.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="timeoutMilliseconds">The timeout in milliseconds.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains true if the task completed successfully; otherwise, false.</returns>
    public async Task<bool> ExecuteWithTimeoutAsync(Action action, int timeoutMilliseconds)
    {
        var cancellationToken = CreateToken(timeoutMilliseconds);
        return await ExecuteWithCancellationAsync(action, cancellationToken);
    }
    
    
}