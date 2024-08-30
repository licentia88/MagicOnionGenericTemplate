namespace MagicT.Server.Handlers;


//[DelayRequestFilter(1)]
[global::RegisterSingleton]
public class MyAsyncRequestHandler : global::MessagePipe.IAsyncRequestHandler<int, string>
{
    public global::System.Threading.Tasks.ValueTask<string> InvokeAsync(int request, global::System.Threading.CancellationToken cancellationToken = default)
    {
        return global::System.Threading.Tasks.ValueTask.FromResult("Do something here");
    }
}

 

