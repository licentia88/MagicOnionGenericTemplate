using Generator.Components.Args;
using Generator.Components.Interfaces;
using MagicT.Client.Services.Base;
using MagicT.Shared.Enums;
using MagicT.Shared.Extensions;
using MagicT.Shared.Services.Base;
using MagicT.Web.Extensions;
using MagicT.Web.Models;
using MagicT.Web.Pages.HelperComponents;
using MemoryPack;
using MessagePipe;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MagicT.Web.Pages.Base;

public abstract class ServicePageBase<TModel, TService> : PageBaseClass
    where TModel : new()
    where TService : IMagicService<TService, TModel>
{
    protected IGenView<TModel> View;

    [CascadingParameter(Name = nameof(PublicKey))]
    protected byte[] PublicKey { get; set; }

    

    [Inject] protected TService Service { get; set; }

    [Inject] protected List<TModel> DataSource { get; set; } = new();

    [Inject] public ISubscriber<Operation, TModel> Subscriber { get; set; }

    public MagicClientSecureServiceBase<TService, TModel> SecureService =>
        Service as MagicClientSecureServiceBase<TService, TModel>;

 

    protected override Task OnInitializedAsync()
    {
        Subscriber.Subscribe(Operation.Create, _ => InvokeAsync(StateHasChanged));
        Subscriber.Subscribe(Operation.Read, _ => InvokeAsync(StateHasChanged));
        Subscriber.Subscribe(Operation.Update, _ => InvokeAsync(StateHasChanged));
        Subscriber.Subscribe(Operation.Delete, _ => InvokeAsync(StateHasChanged));
        Subscriber.Subscribe(Operation.Stream, _ => InvokeAsync(StateHasChanged));

        return base.OnInitializedAsync();
    }
 
 

    protected virtual async Task<TModel> Create(GenArgs<TModel> args)
    {
       return  await ExecuteAsync(async () =>
        {
            var result = await Service.Create(args.Model);

            var primaryKey = args.Model.GetPrimaryKey();

            args.Model.SetPropertyValue(primaryKey, result.GetPropertyValue(primaryKey));

            args.Model = result;

            DataSource.Add(result);

            return result;
        });
    }

    protected virtual async Task<TModel> CreateEncrypted(GenArgs<TModel> args)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await SecureService.CreateEncrypted(args.Model);

            var primaryKey = args.Model.GetPrimaryKey();

            args.Model.SetPropertyValue(primaryKey, result.GetPropertyValue(primaryKey));

            args.Model = result;

            DataSource.Add(result);

            return result;
        });
    }

    protected virtual async Task<List<TModel>> Read(SearchArgs args)
    {
       return  await ExecuteAsync(async () =>
        {
            var result = await Service.Read();

            DataSource = result;

            return result;
        });
    }

    protected virtual async Task<List<TModel>> ReadAlEncrypted(SearchArgs args)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await SecureService.ReadEncrypted();

            DataSource = result;

            return result;
        });
    }

    protected virtual async Task<TModel> Update(GenArgs<TModel> args)
    {
        return await ExecuteAsync(async () =>
       {
           var result = await Service.Update(args.Model);

           return result;
       }).OnComplete((TModel arg1) =>
       {
           if (arg1 is null) return;

           var index = DataSource.IndexOf(args.Model);

           DataSource[index] = args.OldModel;

       });

        //Datasource da mevcut Datayi replace yap
    }

   

    protected virtual async Task<TModel> UpdateEncrypted(GenArgs<TModel> args)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await SecureService.UpdateEncrypted(args.Model);

            return result;
        }).OnComplete((TModel arg) =>
        {
            if (arg is null) return;
            var index = DataSource.IndexOf(args.Model);
            DataSource[index] = args.OldModel;
        });

        //Datasource da mevcut Datayi replace yap
    }

    protected virtual async Task<TModel> Delete(GenArgs<TModel> args)
    {
        var Dialog = await DialogService.ShowAsync<ConfirmDelete>("Confirm Delete");

        var dialogResult = await Dialog.Result;

        if (dialogResult.Cancelled)
        {
            NotificationsView.Notifications.Add(new NotificationVM("Cancelled", Severity.Info));
            NotificationsView.Fire();
            return args.OldModel;
        }

        return await ExecuteAsync(async () =>
        {
            var result = await Service.Delete(args.Model);

            DataSource.Remove(args.Model);

            return result;
        });
    }


    protected virtual async Task<List<TModel>> FindByParameters(params KeyValuePair<string, object>[] parameters)
    {
        return  await ExecuteAsync(async () =>
        {
            var paramBytes = parameters.PickleToBytes();

            var result = await Service.FindByParameters(paramBytes);

            DataSource = result;

            return result;
        });
    }

    protected virtual async Task<TModel> DeleteEncrypted(GenArgs<TModel> args)
    {
        var Dialog = await DialogService.ShowAsync<ConfirmDelete>("Confirm Delete");

        var dialogResult = await Dialog.Result;

        if (dialogResult.Cancelled)
        {
            NotificationsView.Notifications.Add(new NotificationVM("Cancelled", Severity.Info));
            NotificationsView.Fire();
            return args.OldModel;
        }

        return await ExecuteAsync(async () =>
        {
            var result = await SecureService.DeleteEncrypted(args.Model);

            DataSource.Remove(args.Model);

            return result;
        });
    }


    protected virtual void Cancel(GenArgs<TModel> args)
    {
        Execute(() =>
        {
            DataSource[args.Index] = args.OldModel;
            return true;
        });
    }

    protected virtual Task Load(IGenView<TModel> view)
    {
        View = view;

        return Task.CompletedTask;
    }

   
    /// <summary>
    /// Gets Concrete Service of the type
    /// </summary>
    /// <typeparam name="TCService"></typeparam>
    /// <returns></returns>
    //public TCService GetService<TCService>() where TCService : MagicClientServiceBase<TService, TModel> => Service as TCService;
}

public abstract class ServicePageBase<TModel, TChild, TService> : ServicePageBase<TChild, TService>
    where TService : IMagicService<TService, TChild>
    where TModel : new()
    where TChild : new()
{
    [Parameter] public TModel ParentModel { get; set; }


    protected override Task<TChild> Create(GenArgs<TChild> args)
    {
        var pk = ParentModel.GetPrimaryKey();

        var fk = ModelExtensions.GetForeignKey<TModel, TChild>();

        args.Model.SetPropertyValue(fk, ParentModel.GetPropertyValue(pk));

        return base.Create(args);
    }

    protected override Task<TChild> CreateEncrypted(GenArgs<TChild> args)
    {
        var pk = ParentModel.GetPrimaryKey();

        var fk = ModelExtensions.GetForeignKey<TModel, TChild>();

        args.Model.SetPropertyValue(fk, ParentModel.GetPropertyValue(pk));

        return base.CreateEncrypted(args);
    }

    protected virtual async Task<List<TChild>> FindByParent()
    {
        return await ExecuteAsync(async () =>
        {
            var pk = ParentModel.GetPrimaryKey();

            var fk = ModelExtensions.GetForeignKey<TModel, TChild>();
            var result = await Service.FindByParent(ParentModel.GetPropertyValue(pk).ToString(), fk);

            DataSource = result;
            return result;
        });
    }

    protected virtual async Task<List<TChild>> FindByParentEncrypted()
    {
        return  await ExecuteAsync(async () =>
        {
            var pk = ParentModel.GetPrimaryKey();

            var fk = ModelExtensions.GetForeignKey<TModel, TChild>();
            var result = await SecureService.FindByParentEncrypted(ParentModel.GetPropertyValue(pk).ToString(), fk);

            DataSource = result;
            return result;
        });
    }


}