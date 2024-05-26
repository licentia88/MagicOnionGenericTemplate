using Generator.Components.Args;
using Generator.Components.Interfaces;
using MagicT.Client.Hubs.Base;
using MagicT.Shared.Enums;
using MagicT.Shared.Extensions;
using MagicT.Shared.Hubs.Base;
using MagicT.Web.Shared.Extensions;
using MagicT.Web.Shared.HelperComponents;
using MagicT.Web.Shared.Models;
using MessagePipe;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MagicT.Web.Shared.Base;

public abstract class HubPageBase<THub, ITHub, THubReceiver, TModel> : PageBaseClass
    where TModel : class, new()
    where THub : MagicHubClientBase<ITHub, THubReceiver, TModel>
    where ITHub : IMagicHub<ITHub, THubReceiver, TModel>
    where THubReceiver : class, IMagicReceiver<TModel>
{
    public IGenGrid<TModel> Grid;

    protected IGenView<TModel> View;
    [Inject] protected ITHub IService { get; set; }

    protected List<TModel> DataSource
    {
        get => Service.Collection;
        set => Service.Collection = value;
    }

    [Inject]
    public virtual ISubscriber<Operation, TModel> Subscriber { get; set; }

    [Inject]
    public virtual ISubscriber<Operation, List<TModel>> ListSubscriber { get; set; }

    protected THub Service => IService as THub;

    protected override Task OnInitializedAsync()
    {
        Subscriber.Subscribe(Operation.Create, model => InvokeAsync(StateHasChanged));
        Subscriber.Subscribe(Operation.Read, model => InvokeAsync(StateHasChanged));
        Subscriber.Subscribe(Operation.Update, model => InvokeAsync(StateHasChanged));
        Subscriber.Subscribe(Operation.Delete, model => InvokeAsync(StateHasChanged));
        ListSubscriber.Subscribe(Operation.Stream, model => InvokeAsync(StateHasChanged));

        return base.OnInitializedAsync();
    }

    protected virtual async Task<TModel> CreateAsync(GenArgs<TModel> args)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await Service.CreateAsync(args.CurrentValue);

            var primaryKey = args.CurrentValue.GetPrimaryKey();

            args.CurrentValue.SetPropertyValue(primaryKey, result.GetPropertyValue(primaryKey));

            args.CurrentValue = result;

            DataSource.Add(result);

            return result;

        }).OnComplete((_, result) =>
        {
            if (result != TaskResult.Fail) return;
            NotificationsView.Notifications.Add(new NotificationVM("Failed to save", Severity.Error));
            NotificationsView.Fire();

        });
    }

    protected virtual async Task ReadAsync(SearchArgs args)
    {
        await ExecuteAsync(Service.ReadAsync);
    }

    protected virtual async Task<TModel> UpdateAsync(GenArgs<TModel> args)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await Service.UpdateAsync(args.CurrentValue);

            var index = DataSource.IndexByKey(args.CurrentValue);

            DataSource[index] = result;

            args.CurrentValue = result;

            return result;
        }).OnComplete((data, result) =>
        {
            if (result == TaskResult.Success) return Task.FromResult(data);

            //data is null when methodbody fails.
            //Replace the items with existing values
            var index = DataSource.IndexByKey(args.CurrentValue);

            DataSource[index] = args.OldValue;

            return Task.FromResult(data);
        });
    }

    protected virtual async Task<TModel> DeleteAsync(GenArgs<TModel> args)
    {
        var Dialog = await DialogService.ShowAsync<ConfirmDelete>("Confirm Delete");

        var dialogResult = await Dialog.Result;

        if (!(bool)dialogResult.Data)
        {
            NotificationsView.Notifications.Add(new NotificationVM("Cancelled", Severity.Info));
            NotificationsView.Fire();
            return args.OldValue;
        }

        return await ExecuteAsync(async () =>
        {
            var result = await Service.DeleteAsync(args.CurrentValue);

            DataSource.Remove(args.CurrentValue);

            return result;
        });
    }

    protected virtual async Task<List<TModel>> FindByParametersAsync(SearchArgs args)
    {
        return await ExecuteAsync(async () =>
        {
            KeyValuePair<string, object>[] parameters = args.WhereStatements.Where(x => x.Value is not null).ToArray();

            byte[] paramBytes = null;

            if (parameters.Any())
                paramBytes = parameters.SerializeToBytes();

            //var test = parameters.SerializeToBytes();

            var result = await Service.FindByParametersAsync(paramBytes);

            DataSource = result;

            return result;
        });
    }


    protected virtual void Cancel(GenArgs<TModel> args)
    {
        Execute(() =>
        {
            DataSource[args.Index] = args.OldValue;
            return true;
        });
    }

    protected virtual Task LoadAsync(IGenView<TModel> view)
    {
        View = view;

        return Task.CompletedTask;
    }
}

public abstract class HubPageBase<THub, ITHub, THubReceiver, TModel, TChild> : HubPageBase<THub, ITHub, THubReceiver, TChild>
    where TModel : class, new()
    where TChild : class, new()
    where THub : MagicHubClientBase<ITHub, THubReceiver, TChild>
    where ITHub : IMagicHub<ITHub, THubReceiver, TChild>
    where THubReceiver : class, IMagicReceiver<TChild>
{

    [Parameter, EditorRequired]
    public TModel ParentModel { get; set; }

    protected override async Task OnBeforeInitializeAsync()
    {
        await base.OnBeforeInitializeAsync();

        await FindByParentAsync();
    }
    protected override Task<TChild> CreateAsync(GenArgs<TChild> args)
    {
        if (ParentModel is null) return base.CreateAsync(args);

        var pk = ParentModel.GetPrimaryKey();

        var fk = ModelExtensions.GetForeignKey<TModel, TChild>();

        args.CurrentValue.SetPropertyValue(fk, ParentModel.GetPropertyValue(pk));

        return base.CreateAsync(args);
    }


    protected virtual async Task<List<TChild>> FindByParentAsync()
    {
        return await ExecuteAsync(async () =>
        {
            var pk = ParentModel.GetPrimaryKey();

            var fk = ModelExtensions.GetForeignKey<TModel, TChild>();

            var fkValue = ParentModel.GetPropertyValue(pk)?.ToString();

            var result = await Service.FindByParentAsync(fkValue, fk);

            DataSource = result;
            return result;
        });
    }

}