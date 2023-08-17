namespace MagicT.Server.BackgroundTasks;

public class QueuedHostedService : BackgroundService
{
    readonly ILogger<QueuedHostedService> _logger;

    public IBackGroundTaskQueue TaskQueue { get; set; }

    public QueuedHostedService(IBackGroundTaskQueue taskQueue, ILogger<QueuedHostedService> logger)
    {
        TaskQueue = taskQueue;
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        //TODO:Add log
        await BackGroundProcessing(cancellationToken);
    }

    private async Task BackGroundProcessing(CancellationToken cancellationToken)
    {
        //TODO:Use try catch and log
        while (!cancellationToken.IsCancellationRequested)
        {
            var work = await TaskQueue.DequeAsync(cancellationToken);

            await work(cancellationToken);
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        //TODO:Add log
        return base.StopAsync(cancellationToken);
    }
}
