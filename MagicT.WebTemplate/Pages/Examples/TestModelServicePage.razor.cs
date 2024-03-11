using Blazored.LocalStorage;
using Generator.Components.Args;
using Grpc.Core;
using MagicT.Shared.Enums;
using MagicT.Shared.Models;
using MagicT.Shared.Services;
using MagicT.WebTemplate.Extensions;
using Microsoft.AspNetCore.Components;

namespace MagicT.WebTemplate.Pages.Examples;

public sealed partial class TestModelServicePage
{
    [Inject]
    public ITestService TestService { get; set; }

    protected override async Task<List<TestModel>> ReadAsync(SearchArgs args)
    {
        var response = await Service.StreamReadAllAsync(100).ConfigureAwait(false);

        await foreach (var dataList in response.ResponseStream.ReadAllAsync())
        {
            DataSource.AddRange(dataList);

            StateHasChanged();
            await Task.Delay(100);

        }

        return DataSource;
    }
    public async Task FailAdd()
    {
        await ExecuteAsync(async () =>
        {
            var result = await TestService.CreateAsync(new TestModel { Id = 2 });

            Console.WriteLine("done");

            return result;
        }).OnComplete((TestModel model, TaskResult arg) =>
        {
            Console.WriteLine("");
        });

    }

    public async Task AddMillionData()
    {
        await TestService.CreateMillionsData();

        Console.WriteLine("done");

    }
}

 