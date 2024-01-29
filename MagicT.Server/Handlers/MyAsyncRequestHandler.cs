using MessagePipe;

namespace MagicT.Server.Handlers;


//[DelayRequestFilter(1)]
[RegisterSingleton]
public class MyAsyncRequestHandler : IAsyncRequestHandler<int, string>
{
    public  ValueTask<string> InvokeAsync(int request, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult("Do something here");
    }
}

 

