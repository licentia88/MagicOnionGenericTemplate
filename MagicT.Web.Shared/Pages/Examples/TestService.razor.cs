using Microsoft.AspNetCore.Components;

namespace MagicT.Web.Shared.Pages.Examples;

public partial class TestService
{
	public List<TestModel> DataSource { get; set; } = new();

    [Inject]
    public ITestService ITestService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        DataSource = await ITestService.ReadAsync();
        await base.OnInitializedAsync();
    }

   
}

