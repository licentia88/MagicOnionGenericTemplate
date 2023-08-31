namespace MagicT.Server.HostedServices;

public interface IBackGroundTaskQueue
{
    IServiceProvider Provider { get; }

    Task QueueBackGroundWorKAsync(Func<CancellationToken, Task> work);

    Task<Func<CancellationToken, Task>> DequeAsync(CancellationToken cancellationToken);
}
