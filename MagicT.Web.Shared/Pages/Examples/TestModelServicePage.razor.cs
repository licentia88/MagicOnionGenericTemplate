using Generator.Components.Args;
using Grpc.Core;
using MagicT.Shared.Enums;
using MagicT.Web.Shared.Extensions;
using Microsoft.AspNetCore.Components;

namespace MagicT.Web.Shared.Pages.Examples;

public sealed partial class TestModelServicePage
{
    [Inject]
    public ITestService TestService { get; set; }

    protected override async Task<List<TestModel>> ReadAsync(SearchArgs args)
    {

        var data = await Service.ReadAsync();

        var firstData = data.FirstOrDefault();

        Service.UpdateAsync(firstData);

        Service.UpdateAsync(firstData);


        Console.WriteLine();
        //var response = await Service.StreamReadAllAsync(10000);

        //await foreach (var dataList in response.ResponseStream.ReadAllAsync())
        //{
        //    DataSource.AddRange(dataList);

        //    StateHasChanged();
        //    await Task.Delay(100);

        //}

        return DataSource;
    }


    public async Task FailAdd()
    {
        await ExecuteAsync(async () =>
        {
            var result = await TestService.CreateAsync(new TestModel { Description="test", DescriptionDetails="444444" });

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

    protected override async Task<TestModel> CreateAsync(GenArgs<TestModel> args)
    {
        await Task.Delay(3000);
        var responseModel = await base.CreateAsync(args);


        return responseModel;
    }

}

 