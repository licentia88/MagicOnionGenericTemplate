using Generator.Components.Args;
using MagicT.Shared.Enums;
using MagicT.Shared.Extensions;
using MagicT.Shared.Services.Base;
using MagicT.WebTemplate.Components.HelperComponents;
using MagicT.WebTemplate.Extensions;
using MagicT.WebTemplate.Models;
using MessagePipe;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MagicT.WebTemplate.Pages.Base;


public abstract class ServiceSecurePageBase<TModel, TService> : ServicePageBase<TModel, TService>
    where TModel : class, new()
    where TService : ISecureMagicService<TService, TModel>//, ISecureClientM<TModel>
{
    protected override Task LoadAsync()
    {
        Subscriber.Subscribe(Operation.Create, _ => InvokeAsync(StateHasChanged));
        Subscriber.Subscribe(Operation.Read, _ => InvokeAsync(StateHasChanged));
        Subscriber.Subscribe(Operation.Update, _ => InvokeAsync(StateHasChanged));
        Subscriber.Subscribe(Operation.Delete, _ => InvokeAsync(StateHasChanged));
        Subscriber.Subscribe(Operation.Stream, _ => InvokeAsync(StateHasChanged));

        return base.LoadAsync();
    }


    protected virtual async Task<TModel> CreateEncryptedAsync(GenArgs<TModel> args)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await ((ISecureClientM<TModel>)Service).CreateEncryptedAsync(args.CurrentValue);

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



    protected virtual async Task<List<TModel>> ReadEncryptedAsync(SearchArgs args)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await ((ISecureClientM<TModel>)Service).ReadEncryptedAsync();

            DataSource = result;

            return result;
        });
    }



    protected virtual async Task<TModel> UpdateEncryptedAsync(GenArgs<TModel> args)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await ((ISecureClientM<TModel>)Service).UpdateEncryptedAsync(args.CurrentValue);

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
            var result = await ((ISecureClientM<TModel>)Service).DeleteEncryptedAsync(args.CurrentValue);

            DataSource.Remove(args.CurrentValue);

            return result;
        });
    }


    protected virtual async Task<List<TModel>> FindByParametersEncryptedAsync(SearchArgs args)
    {
        return await ExecuteAsync(async () =>
        {
            KeyValuePair<string, object>[] parameters = args.WhereStatements.Where(x => x.Value is not null).ToArray();

            byte[] paramBytes = null;

            if (parameters.Any())
                paramBytes = parameters.SerializeToBytes();

            //var test = parameters.SerializeToBytes();

            var result = await ((ISecureClientM<TModel>)Service).FindByParametersEncryptedAsync(paramBytes);

            DataSource = result;

            return result;
        });
    }

}

public abstract class ServiceSecurePageBase<TModel, TChild, TService> : ServiceSecurePageBase<TChild, TService>
    where TService : ISecureMagicService<TService, TChild>
    where TModel : new()
    where TChild : class, new()
{
    [Parameter, EditorRequired] public TModel ParentModel { get; set; }

    protected override Task<TChild> CreateEncryptedAsync(GenArgs<TChild> args)
    {
        var pk = ParentModel.GetPrimaryKey();

        var fk = ModelExtensions.GetForeignKey<TModel, TChild>();

        args.CurrentValue.SetPropertyValue(fk, ParentModel.GetPropertyValue(pk));

        return base.CreateEncryptedAsync(args);
    }

    protected virtual async Task<List<TChild>> FindByParentEncryptedAsync()
    {
        return await ExecuteAsync(async () =>
        {
            var pk = ParentModel.GetPrimaryKey();

            var fk = ModelExtensions.GetForeignKey<TModel, TChild>();

            var fkValue = ParentModel.GetPropertyValue(pk)?.ToString();

            var result = await ((ISecureClientM<TChild>)Service).FindByParentEncryptedAsync(fkValue, fk);

            DataSource = result;
            return result;
        });
    }




}