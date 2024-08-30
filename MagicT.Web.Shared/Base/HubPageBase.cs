using Generator.Components.Args;
using Generator.Components.Interfaces;
using MagicT.Client.Hubs.Base;
using MagicT.Shared.Enums;
using MagicT.Shared.Extensions;
using MagicT.Shared.Hubs.Base;
using MagicT.Web.Shared.Extensions;
using MagicT.Web.Shared.Models;
using MagicT.Web.Shared.Pages.Shared;
using MessagePipe;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MagicT.Web.Shared.Base;

/// <summary>
/// Abstract base class for pages that communicate with a hub.
/// </summary>
/// <typeparam name="THub">The type of the hub client.</typeparam>
/// <typeparam name="ITHub">The interface type of the hub.</typeparam>
/// <typeparam name="THubReceiver">The type of the hub receiver.</typeparam>
/// <typeparam name="TModel">The type of the model.</typeparam>
public abstract class HubPageBase<THub, ITHub, THubReceiver, TModel> : PageBaseClass
    where TModel : class, new()
    where THub : MagicHubClientBase<ITHub, THubReceiver, TModel>
    where ITHub : IMagicHub<ITHub, THubReceiver, TModel>
    where THubReceiver : class, IMagicReceiver<TModel>
{
    /// <summary>
    /// The grid component displaying the data.
    /// </summary>
    public IGenGrid<TModel> Grid;

    /// <summary>
    /// The view component for the data.
    /// </summary>
    protected IGenView<TModel> View;

    /// <summary>
    /// Gets or sets the service that communicates with the hub.
    /// </summary>
    [Inject] protected ITHub IService { get; set; }

    /// <summary>
    /// Gets or sets the data source for the grid and view.
    /// </summary>
    protected List<TModel> DataSource
    {
        get => Service.Collection;
        set => Service.Collection = value;
    }

    /// <summary>
    /// Subscriber for individual model operations.
    /// </summary>
    [Inject] public virtual ISubscriber<Operation, TModel> Subscriber { get; set; }

    /// <summary>
    /// Subscriber for list operations.
    /// </summary>
    [Inject] public virtual ISubscriber<Operation, List<TModel>> ListSubscriber { get; set; }

    /// <summary>
    /// Gets the service cast to the hub type.
    /// </summary>
    protected THub Service => IService as THub;

    /// <summary>
    /// Initializes the component.
    /// </summary>
    protected override Task OnInitializedAsync()
    {
        // Subscribing to various operations
        Subscriber.Subscribe(Operation.Create, model => InvokeAsync(StateHasChanged));
        Subscriber.Subscribe(Operation.Read, model => InvokeAsync(StateHasChanged));
        Subscriber.Subscribe(Operation.Update, model => InvokeAsync(StateHasChanged));
        Subscriber.Subscribe(Operation.Delete, model => InvokeAsync(StateHasChanged));
        ListSubscriber.Subscribe(Operation.Stream, model => InvokeAsync(StateHasChanged));

        return base.OnInitializedAsync();
    }

    /// <summary>
    /// Creates a new model.
    /// </summary>
    /// <param name="args">The arguments for the generation.</param>
    /// <returns>The created model.</returns>
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

    /// <summary>
    /// Reads data based on the provided search arguments.
    /// </summary>
    /// <param name="args">The search arguments.</param>
    protected virtual async Task ReadAsync(SearchArgs args)
    {
        await ExecuteAsync(Service.ReadAsync);
    }

    /// <summary>
    /// Updates an existing model.
    /// </summary>
    /// <param name="args">The arguments for the generation.</param>
    /// <returns>The updated model.</returns>
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

            // Replace the items with existing values if update fails
            var index = DataSource.IndexByKey(args.CurrentValue);
            DataSource[index] = args.OldValue;

            return Task.FromResult(data);
        });
    }

    /// <summary>
    /// Deletes an existing model.
    /// </summary>
    /// <param name="args">The arguments for the generation.</param>
    /// <returns>The deleted model.</returns>
    protected virtual async Task<TModel> DeleteAsync(GenArgs<TModel> args)
    {
        var dialog = await DialogService.ShowAsync<ConfirmDelete>("Confirm Delete");
        var dialogResult = await dialog.Result;

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

    /// <summary>
    /// Finds models based on the provided search parameters.
    /// </summary>
    /// <param name="args">The search arguments.</param>
    /// <returns>A list of models matching the search parameters.</returns>
    protected virtual async Task<List<TModel>> FindByParametersAsync(SearchArgs args)
    {
        return await ExecuteAsync(async () =>
        {
            KeyValuePair<string, object>[] parameters = args.WhereStatements.Where(x => x.Value is not null).ToArray();
            byte[] paramBytes = null;

            if (parameters.Any())
                paramBytes = parameters.SerializeToBytes();

            var result = await Service.FindByParametersAsync(paramBytes);
            DataSource = result;

            return result;
        });
    }

    /// <summary>
    /// Cancels the current operation and reverts the model to its old value.
    /// </summary>
    /// <param name="args">The arguments for the generation.</param>
    protected virtual void Cancel(GenArgs<TModel> args)
    {
        Execute(() =>
        {
            DataSource[args.Index] = args.OldValue;
            return true;
        });
    }

    /// <summary>
    /// Loads the view component.
    /// </summary>
    /// <param name="view">The view component to load.</param>
    /// <returns>A task representing the load operation.</returns>
    protected virtual Task LoadAsync(IGenView<TModel> view)
    {
        View = view;
        return Task.CompletedTask;
    }
}

/// <summary>
/// Abstract base class for pages that communicate with a hub and handle a parent-child relationship.
/// </summary>
/// <typeparam name="THub">The type of the hub client.</typeparam>
/// <typeparam name="ITHub">The interface type of the hub.</typeparam>
/// <typeparam name="THubReceiver">The type of the hub receiver.</typeparam>
/// <typeparam name="TModel">The type of the parent model.</typeparam>
/// <typeparam name="TChild">The type of the child model.</typeparam>
public abstract class HubPageBase<THub, ITHub, THubReceiver, TModel, TChild> : HubPageBase<THub, ITHub, THubReceiver, TChild>
    where TModel : class, new()
    where TChild : class, new()
    where THub : MagicHubClientBase<ITHub, THubReceiver, TChild>
    where ITHub : IMagicHub<ITHub, THubReceiver, TChild>
    where THubReceiver : class, IMagicReceiver<TChild>
{
    /// <summary>
    /// Gets or sets the parent model.
    /// </summary>
    [Parameter, EditorRequired]
    public TModel ParentModel { get; set; }

    /// <summary>
    /// Initializes the component before the main initialization.
    /// </summary>
    protected override async Task OnBeforeInitializeAsync()
    {
        await base.OnBeforeInitializeAsync();
        await FindByParentAsync();
    }

    /// <summary>
    /// Creates a new child model.
    /// </summary>
    /// <param name="args">The arguments for the generation.</param>
    /// <returns>The created child model.</returns>
    protected override Task<TChild> CreateAsync(GenArgs<TChild> args)
    {
        if (ParentModel is null) return base.CreateAsync(args);

        var pk = ParentModel.GetPrimaryKey();
        var fk = ModelExtensions.GetForeignKey<TModel, TChild>();

        args.CurrentValue.SetPropertyValue(fk, ParentModel.GetPropertyValue(pk));

        return base.CreateAsync(args);
    }

    /// <summary>
    /// Finds child models based on the parent model.
    /// </summary>
    /// <returns>A list of child models associated with the parent model.</returns>
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
