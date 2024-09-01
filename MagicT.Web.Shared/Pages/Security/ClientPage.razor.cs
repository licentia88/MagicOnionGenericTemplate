using Microsoft.AspNetCore.Components;

namespace MagicT.Web.Shared.Pages.Security;

public sealed partial class ClientPage
{
    [Inject]
    public IClientBlockService Service { get; set; }

    public List<ClientData> DataSource { get; set; }


    protected override async Task OnInitializedAsync()
    {
        await ExecuteAsync(async () =>
        {
            var response = await Service.ReadClients();

            DataSource = response;
        });
        await base.OnInitializedAsync();
    }
}

 