using Blazored.LocalStorage;
using Grpc.Core;
using MagicT.Shared.Enums;
using MagicT.Shared.Helpers;
using MagicT.Shared.Models;
using MagicT.Shared.Services;
using MagicT.Web.Extensions;
using Microsoft.AspNetCore.Components;

namespace MagicT.Web.Pages.Examples;

public sealed partial class TestModelServicePage
{
    [Inject]
    public ITestService TestService { get; set; }

    [Inject]
    ILocalStorageService storageService { get; set; }

    protected override async Task ShowAsync()
    {
        //var stream = await Service.StreamReadAllAsync(3);

        //await foreach (var x in stream.ResponseStream.ReadAllAsync())
        //{
        //    foreach (var item in x)
        //    {
        //        Console.WriteLine(item.Description);
        //    }
        //}


        await ExecuteAsync(async () =>
        {
            var sharedbin = await storageService.GetItemAsync<byte[]>("shared-bin");

            var encryptedData = CryptoHelper.EncryptData("ASIM", sharedbin);

            var test = await Service.EncryptedString(encryptedData);

        });
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
}

 