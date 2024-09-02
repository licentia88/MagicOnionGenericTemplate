



using Generator.Components.Interfaces;
using MagicT.Redis.Models;
using Microsoft.AspNetCore.Components;

namespace MagicT.Web.Shared.Pages.Security;

public sealed partial class ClientPage
{
    [Inject]
    public IClientBlockService Service { get; set; }

    public List<ClientData> DataSource { get; set; }

    public IGenView<ClientData> View { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await ExecuteAsync(async () =>
        {
            var response = await Service.ReadClients();

            DataSource = response;
        });
        await base.OnInitializedAsync();
    }

    private async Task Lock(object data)
    {
        if(data is not ClientData clientData)return;
        
        await ExecuteAsync(async () =>
        {
            var response = await Service.AddHardBlock(clientData);

            var index = DataSource.IndexOf(clientData);
            
            DataSource[index] = response;

        });
    }

    private async Task UnLock(object data)
    {
        if(data is not ClientData clientData) return;
        
        await ExecuteAsync(async () =>
        {
            await Service.RemovePermanentBlock(clientData);

                var index = DataSource.IndexOf(clientData);
            
            DataSource.RemoveAt(index);

        });
    }

    private void OnLoad(IGenView<ClientData> obj)
    {
        View = obj;
    }
}

 