using Generator.Components.Args;
using Generator.Components.Interfaces;
using MagicT.Client.Hubs.Base;
using MagicT.Shared.Enums;
using MagicT.Shared.Hubs.Base;
using MagicT.WebTemplate.Components.HelperComponents;
using MagicT.WebTemplate.Models;
using MessagePipe;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MagicT.WebTemplate.Pages.Base;

public abstract class HubPageBase<THub, ITHub, THubReceiver, TModel> : PageBaseClass
    where TModel : new()
    where THub : MagicHubClientBase<ITHub, THubReceiver, TModel>
    where ITHub : IMagicHub<ITHub, THubReceiver, TModel>
    where THubReceiver : class, IMagicReceiver<TModel>
{

    [Inject] protected ITHub IService { get; set; }

    protected List<TModel> DataSource => Service.Collection;

    [Inject]
    public virtual ISubscriber<Operation, TModel> Subscriber { get; set; }

    [Inject]
    public virtual ISubscriber<Operation, List<TModel>> ListSubscriber { get; set; }

    protected THub Service => IService as THub;

    protected override Task OnInitializedAsync()
    {
        Subscriber.Subscribe(Operation.Create, model =>
        {
            //TODO burada view parenti set et
            InvokeAsync(StateHasChanged);
        });

        Subscriber.Subscribe(Operation.Read, model => InvokeAsync(StateHasChanged));
        Subscriber.Subscribe(Operation.Update, model => InvokeAsync(StateHasChanged));
        Subscriber.Subscribe(Operation.Delete, model => InvokeAsync(StateHasChanged));
        ListSubscriber.Subscribe(Operation.Stream, model => InvokeAsync(StateHasChanged));

        return base.OnInitializedAsync();
    }


    protected virtual async Task Create(GenArgs<TModel> args)
    {
        await ExecuteAsync(async () =>
        {
            await Service.CreateAsync(args.CurrentValue);

            // var primaryKey = args.CurrentValue.GetPrimaryKey();
            //
            // args.CurrentValue.SetPropertyValue(primaryKey, result.GetPropertyValue(primaryKey));
            //
            // args.CurrentValue = result;
            //
            // return result;
        });
    }

    protected virtual async Task Read(SearchArgs args)
    {
        await ExecuteAsync(Service.ReadAsync);
    }

    protected virtual async Task Update(GenArgs<TModel> args)
    {
        await ExecuteAsync(async () => { await Service.UpdateAsync(args.CurrentValue); });
    }

    protected virtual async Task Delete(GenArgs<TModel> args)
    {
        var Dialog = await DialogService.ShowAsync<ConfirmDelete>("Confirm Delete");

        var dialogResult = await Dialog.Result;

        if (dialogResult.Cancelled)
            NotificationsView.Notifications.Add(new NotificationVM("Cancelled", Severity.Info));

        await ExecuteAsync(async () => { await Service.DeleteAsync(args.CurrentValue); });
    }

    protected virtual void Cancel(GenArgs<TModel> args)
    {
        DataSource[args.Index] = args.OldValue;
    }

    protected virtual Task Load(IGenView<TModel> View)
    {
        return Task.CompletedTask;
    }
}