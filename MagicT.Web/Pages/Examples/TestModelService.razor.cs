using Blazored.LocalStorage;
using MagicT.Shared.Enums;
using MagicT.Shared.Helpers;
using MagicT.Shared.Models;
using MagicT.Shared.Services;
using MagicT.Web.Extensions;
using Microsoft.AspNetCore.Components;

namespace MagicT.Web.Pages.Examples;

public sealed partial class TestModelService
{
    [Inject]
    public ITestService TestService { get; set; }

    [Inject]
    ILocalStorageService storageService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

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

 