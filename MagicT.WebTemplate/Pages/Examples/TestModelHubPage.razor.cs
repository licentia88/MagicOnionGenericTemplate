using MagicT.Shared.Models;

namespace MagicT.WebTemplate.Pages.Examples;

public partial class TestModelHubPage
{

    public async Task OnCreateButton()
    {
        var newData = GenFu.GenFu.New<TestModel>();
        newData.Id = 0;

        await Service.CreateAsync(newData);
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

       
        //await Service.ReadAsync();
    }
}

