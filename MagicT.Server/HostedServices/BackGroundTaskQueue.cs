using System.Threading.Channels;

namespace MagicT.Server.HostedServices;

public class BackGroundTaskQueue : IBackGroundTaskQueue
{
    public IServiceProvider Provider { get; }

    private readonly Channel<Func<CancellationToken, Task>> _queue;

    public BackGroundTaskQueue(int capacity, IServiceCollection services)
    {
        Provider = services.BuildServiceProvider();

        _queue = Channel.CreateBounded<Func<CancellationToken, Task>>(new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        });
    }

    public async Task<Func<CancellationToken, Task>> DequeAsync(CancellationToken cancellationToken)
    {
        return await _queue.Reader.ReadAsync(cancellationToken);

    }

    public async Task QueueBackGroundWorKAsync(Func<CancellationToken, Task> work)
    {
        if (work is null) throw new ArgumentNullException(nameof(work));

        await _queue.Writer.WriteAsync(work);
    }
}
