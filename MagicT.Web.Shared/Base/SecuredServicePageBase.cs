using Generator.Components.Args;
using MagicT.Client.Services.Base;
using MagicT.Shared.Enums;
using MagicT.Shared.Extensions;
using MagicT.Shared.Services.Base;
using MagicT.Web.Shared.Extensions;
using MagicT.Web.Shared.Models;
using MagicT.Web.Shared.Pages.Shared;
using MessagePipe;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MagicT.Web.Shared.Base;

/// <summary>
/// An abstract base class for secure service pages.
/// </summary>
/// <typeparam name="TModel">The type of the model.</typeparam>
/// <typeparam name="TService">The type of the service.</typeparam>
public abstract class ServiceSecurePageBase<TModel, TService> : ServicePageBase<TModel, TService>
    where TModel : class, new()
    where TService : IMagicSecureService<TService, TModel>//, ISecureClientM<TModel>
{
    /// <summary>
    /// Overrides the OnBeforeInitializeAsync method to subscribe to various operations and invoke StateHasChanged when necessary.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override Task OnBeforeInitializeAsync()
    {
        Subscriber.Subscribe(Operation.Create, _ => InvokeAsync(StateHasChanged));
        Subscriber.Subscribe(Operation.Read, _ => InvokeAsync(StateHasChanged));
        Subscriber.Subscribe(Operation.Update, _ => InvokeAsync(StateHasChanged));
        Subscriber.Subscribe(Operation.Delete, _ => InvokeAsync(StateHasChanged));
        Subscriber.Subscribe(Operation.Stream, _ => InvokeAsync(StateHasChanged));

        return base.OnBeforeInitializeAsync();
    }

    /// <summary>
    /// Creates an encrypted model asynchronously.
    /// </summary>
    /// <param name="args">The arguments for creating the model.</param>
    /// <returns>The created encrypted model.</returns>
    protected virtual async Task<TModel> CreateEncryptedAsync(GenArgs<TModel> args)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await ((ISecureClientMethods<TModel>)Service).CreateEncryptedAsync(args.CurrentValue);

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
    /// Reads encrypted models asynchronously.
    /// </summary>
    /// <param name="args">The arguments for reading the models.</param>
    /// <returns>A list of encrypted models.</returns>
    protected virtual async Task<List<TModel>> ReadEncryptedAsync(SearchArgs args)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await ((ISecureClientMethods<TModel>)Service).ReadEncryptedAsync();

            DataSource = result;

            return result;
        });
    }

    /// <summary>
    /// Updates an encrypted model asynchronously.
    /// </summary>
    /// <param name="args">The arguments for updating the model.</param>
    /// <returns>The updated encrypted model.</returns>
    protected virtual async Task<TModel> UpdateEncryptedAsync(GenArgs<TModel> args)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await ((ISecureClientMethods<TModel>)Service).UpdateEncryptedAsync(args.CurrentValue);

            var index = DataSource.IndexOf(args.CurrentValue);

            DataSource[index] = result;

            args.CurrentValue = result;

            return result;
        }).OnComplete((data, result) =>
        {
            if (result == TaskResult.Success) return Task.FromResult(data);

            //data is null when methodbody fails.
            //Replace the items with existing values
            var index = DataSource.IndexOf(args.CurrentValue);

            DataSource[index] = args.OldValue;

            return Task.FromResult(data);
        });
    }

    /// <summary>
    /// Deletes an encrypted model asynchronously.
    /// </summary>
    /// <param name="args">The arguments for deleting the model.</param>
    /// <returns>The deleted encrypted model.</returns>
    protected virtual async Task<TModel> DeleteEncryptedAsync(GenArgs<TModel> args)
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
            var result = await ((ISecureClientMethods<TModel>)Service).DeleteEncryptedAsync(args.CurrentValue);

            DataSource.Remove(args.CurrentValue);

            return result;
        });
    }

    /// <summary>
    /// Finds encrypted models by parameters asynchronously.
    /// </summary>
    /// <param name="args">The arguments for finding the models.</param>
    /// <returns>A list of encrypted models matching the parameters.</returns>
    protected virtual async Task<List<TModel>> FindByParametersEncryptedAsync(SearchArgs args)
    {
        return await ExecuteAsync(async () =>
        {
            KeyValuePair<string, object>[] parameters = args.WhereStatements.Where(x => x.Value is not null).ToArray();

            byte[] paramBytes = null;

            if (parameters.Any())
                paramBytes = parameters.SerializeToBytes();

            //var test = parameters.SerializeToBytes();

            var result = await ((ISecureClientMethods<TModel>)Service).FindByParametersEncryptedAsync(paramBytes);

            DataSource = result;

            return result;
        });
    }
}

/// <summary>
/// An abstract base class for secure service pages with a parent model.
/// </summary>
/// <typeparam name="TModel">The type of the parent model.</typeparam>
/// <typeparam name="TChild">The type of the child model.</typeparam>
/// <typeparam name="TService">The type of the service.</typeparam>
public abstract class ServiceSecurePageBase<TModel, TChild, TService> : ServiceSecurePageBase<TChild, TService>
    where TService : IMagicSecureService<TService, TChild>
    where TModel : new()
    where TChild : class, new()
{
    /// <summary>
    /// Gets or sets the parent model.
    /// </summary>
    [Parameter, EditorRequired] public TModel ParentModel { get; set; }

    /// <summary>
    /// Overrides the CreateEncryptedAsync method to set the foreign key for the child model.
    /// </summary>
    /// <param name="args">The arguments for creating the child model.</param>
    /// <returns>The created encrypted child model.</returns>
    protected override Task<TChild> CreateEncryptedAsync(GenArgs<TChild> args)
    {
        var pk = ParentModel.GetPrimaryKey();
        var fk = ModelExtensions.GetForeignKey<TModel, TChild>();
        args.CurrentValue.SetPropertyValue(fk, ParentModel.GetPropertyValue(pk));

        return base.CreateEncryptedAsync(args);
    }

    /// <summary>
    /// Finds encrypted child models by the parent model asynchronously.
    /// </summary>
    /// <returns>A list of encrypted child models related to the parent model.</returns>
    protected virtual async Task<List<TChild>> FindByParentEncryptedAsync()
    {
        return await ExecuteAsync(async () =>
        {
            var pk = ParentModel.GetPrimaryKey();
            var fk = ModelExtensions.GetForeignKey<TModel, TChild>();
            var fkValue = ParentModel.GetPropertyValue(pk)?.ToString();

            var result = await ((ISecureClientMethods<TChild>)Service).FindByParentEncryptedAsync(fkValue, fk);

            DataSource = result;
            return result;
        });
    }
}