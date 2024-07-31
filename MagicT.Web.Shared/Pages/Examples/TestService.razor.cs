using Generator.Components.Args;
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

    protected override async Task<TestModel> CreateAsync(GenArgs<TestModel> args)
    {
        await Task.Delay(3000);
        return await base.CreateAsync(args);
    }
}

