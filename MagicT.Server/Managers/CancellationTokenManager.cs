using Benutomo;

namespace MagicT.Server.Managers;

//[RegisterScoped]
[AutomaticDisposeImpl]
public partial class CancellationTokenManager : IDisposable, IAsyncDisposable
{
    [EnableAutomaticDispose]
    private CancellationTokenSource _cancellationTokenSource;

    private readonly int DefaultTimeOut;

    public CancellationTokenManager(IConfiguration configuration)
    {
        DefaultTimeOut = configuration.GetValue<int>("CancellationTokenTimeOut");
    }

    public CancellationToken CreateToken()
    {
        return CreateToken(DefaultTimeOut);
    }

    public CancellationToken CreateToken(int timeoutMilliseconds)
    {
        _cancellationTokenSource = new CancellationTokenSource(timeoutMilliseconds);
        return _cancellationTokenSource.Token;
    }

    public void CancelToken()
    {
        _cancellationTokenSource?.Cancel();
    }

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

    public async Task<bool> ExecuteWithTimeoutAsync(Action action, int timeoutMilliseconds)
    {
        var cancellationToken = CreateToken(timeoutMilliseconds);
        return await ExecuteWithCancellationAsync(action, cancellationToken);
    }
}
