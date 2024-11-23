using Benutomo;
using DocumentFormat.OpenXml.Drawing.Charts;
using Generator.Components.Args;
using Grpc.Core;
using MagicT.Shared.Enums;
using MagicT.Web.Shared.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace MagicT.Web.Shared.Pages.Examples;

[AutomaticDisposeImpl]
public sealed partial class TestModelServicePage
{
    ~TestModelServicePage()
    {
        Dispose(false);
    }
    
    [Inject]
    public ITestService TestService { get; set; }

    IList<IBrowserFile> files = new List<IBrowserFile>();

    // protected override async Task<List<TestModel>> ReadAsync(SearchArgs args)
    // {
    //     return await ExecuteAsync(async () =>
    //     {
    //         for (int i = 0; i < 7; i++)
    //         {
    //             DataSource = await TestService.ReadAsync();
    //         }
    //
    //         return DataSource;
    //     });
    // }
    protected override async Task<List<TestModel>> ReadAsync(SearchArgs args)
    {
        //var data = await Service.ReadAsync();
    
        //var firstData = data.FirstOrDefault();
    
        //Service.UpdateAsync(firstData);
    
        //Service.UpdateAsync(firstData);
    
        //Console.WriteLine();
        var response = await Service.StreamReadAllAsync(10000);
    
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
            var result = await TestService.CreateAsync(new TestModel { Id = 1,Description="test", DescriptionDetails="444444" });

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

     
    private void UploadFiles2(IBrowserFile file)
    {
        files.Add(file);
        //TODO upload the files to the server
    }
}

 