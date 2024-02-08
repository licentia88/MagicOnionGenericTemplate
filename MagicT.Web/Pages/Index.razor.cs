using MagicT.Shared.Models;
using MessagePipe;

namespace MagicT.Web.Pages;

public partial class Index
{
    //[Inject]
    //IRemoteRequestHandler<int, string> remoteHandler { get; set; }

    //[Inject]
    //IDistributedPublisher<string, USERS> publisher { get; set; }

    //[Inject]
    //IDistributedPublisher<int,string> publisher2 { get; set; }


    protected override async Task OnInitializedAsync()
    {
 
        await base.OnInitializedAsync();

        //await publisher2.PublishAsync(111, "deneme");
    
       
        //await A(remoteHandler);
        //await P(publisher);
    }

  

   
    async Task A(IRemoteRequestHandler<int, string> remoteHandler)
    {
        var v = await remoteHandler.InvokeAsync(9999);
        Console.WriteLine(v); // ECHO:9999
    }

 
    public async Task P(IDistributedPublisher<string, USERS> publisher)
    {
        // publish value to remote process.
        await publisher.PublishAsync("foobar", new USERS { U_FULLNAME = "Licentia"});


    }
}

