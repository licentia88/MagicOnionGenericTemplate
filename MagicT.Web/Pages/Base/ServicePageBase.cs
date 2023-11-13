using Generator.Components.Args;
using Generator.Components.Interfaces;
using MagicT.Client.Services.Base;
using MagicT.Shared.Enums;
using MagicT.Shared.Extensions;
using MagicT.Shared.Services.Base;
using MagicT.Web.Extensions;
using MagicT.Web.Models;
using MagicT.Web.Pages.HelperComponents;
using MessagePipe;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MagicT.Web.Pages.Base;


public abstract class ServicePageBase<TModel, TService> : PageBaseClass
    where TModel : class, new()
    where TService :  IMagicService<TService, TModel>
{
    public IGenGrid<TModel> Grid;

    protected IGenView<TModel> View;

    [CascadingParameter(Name = nameof(PublicKey))]
    protected byte[] PublicKey { get; set; }

    [Inject]
    protected private TService Service { get; set; }

    //[Inject]
    protected List<TModel> DataSource { get; set; } = new();

    [Inject] public ISubscriber<Operation, TModel> Subscriber { get; set; }

    public MagicClientSecureService<TService, TModel> SecureService =>
        Service as MagicClientSecureService<TService, TModel>;

    //public MagicClientServiceBase<TService, TModel> ServiceBase =>
    //   Service as MagicClientServiceBase<TService, TModel>;

 
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
        return await ExecuteAsync(async () =>
        {
            var result = await Service.CreateAsync(args.Model);

            var primaryKey = args.Model.GetPrimaryKey();

            args.Model.SetPropertyValue(primaryKey, result.GetPropertyValue(primaryKey));

            args.Model = result;

            DataSource.Add(result);

            return result;

        }).OnComplete((TModel model, TaskResult result) =>
        {
            if (result == TaskResult.Fail)
            {
                NotificationsView.Notifications.Add(new("Failed to save", Severity.Error));
                NotificationsView.Fire();
            }

        });
    }

    protected virtual async Task<TModel> CreateEncrypted(GenArgs<TModel> args)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await SecureService.CreateEncryptedAsync(args.Model);

            var primaryKey = args.Model.GetPrimaryKey();

            args.Model.SetPropertyValue(primaryKey, result.GetPropertyValue(primaryKey));

            args.Model = result;

            DataSource.Add(result);

            return result;
        });
    }

    protected virtual async Task<List<TModel>> Read(SearchArgs args)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await Service.ReadAsync();

            DataSource = result;

            return result;
        });
    }

    protected virtual async Task<List<TModel>> ReadAlEncrypted(SearchArgs args)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await SecureService.ReadEncryptedAsync();

            DataSource = result;

            return result;
        });
    }

    protected virtual async Task<TModel> Update(GenArgs<TModel> args)
    {
        return await ExecuteAsync(async () =>
       {
           var result = await Service.UpdateAsync(args.Model);

           return result;
       }).OnComplete((TModel data, TaskResult result) =>
       {
           if (result == TaskResult.Success) return Task.FromResult(data);

           //data is null when methodbody fails.
           //Replace the items with existing values
           var index = DataSource.IndexOf(args.Model);

           DataSource[index] = args.OldModel;

           return Task.FromResult(data);
       });
    }

   

    protected virtual async Task<TModel> UpdateEncrypted(GenArgs<TModel> args)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await SecureService.UpdateEncryptedAsync(args.Model);

            return result;
        }).OnComplete((TModel model, TaskResult result) =>
        {
            if (result == TaskResult.Fail)
            {
                var index = DataSource.IndexOf(args.Model);
                DataSource[index] = args.OldModel;
            }
            return Task.FromResult(model);
        });

        //.OnComplete((TModel arg, Tres) =>
        //{
        //    if (arg is null) return;
        //    var index = DataSource.IndexOf(args.Model);
        //    DataSource[index] = args.OldModel;
        //});

        //Datasource da mevcut Datayi replace yap
    }

    protected virtual async Task<TModel> Delete(GenArgs<TModel> args)
    {
        var Dialog = await DialogService.ShowAsync<ConfirmDelete>("Confirm Delete");

        var dialogResult = await Dialog.Result;

        if (!(bool)dialogResult.Data)
        {
            NotificationsView.Notifications.Add(new NotificationVM("Cancelled", Severity.Info));
            NotificationsView.Fire();
            return args.OldModel;
        }

        return await ExecuteAsync(async () =>
        {
            var result = await Service.DeleteAsync(args.Model);
            
            DataSource.Remove(args.Model);

            return result;
        });
    }


    protected virtual async Task<List<TModel>> FindByParametersAsync(SearchArgs args)
    {
        return  await ExecuteAsync(async () =>
        {
            KeyValuePair<string, object>[] parameters = args.WhereStatements.Where(x=> x.Value is not null).ToArray();

            var paramBytes = parameters.PickleToBytes();

            var result = await Service.FindByParametersAsync(paramBytes);

            DataSource = result;

            return result;
        });
    }

    protected virtual async Task<TModel> DeleteEncrypted(GenArgs<TModel> args)
    {
        var Dialog = await DialogService.ShowAsync<ConfirmDelete>("Confirm Delete");

        var dialogResult = await Dialog.Result;

        if (!(bool)dialogResult.Data)
        {
            NotificationsView.Notifications.Add(new NotificationVM("Cancelled", Severity.Info));
            NotificationsView.Fire();
            return args.OldModel;
        }

        return await ExecuteAsync(async () =>
        {
            var result = await SecureService.DeleteEncryptedAsync(args.Model);

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

   
}

public abstract class ServicePageBase<TModel, TChild, TService> : ServicePageBase<TChild, TService>
    where TService : IMagicService<TService, TChild>
    where TModel : new()
    where TChild : class, new()
{
    [Parameter,EditorRequired] public TModel ParentModel { get; set; }


    protected override Task<TChild> Create(GenArgs<TChild> args)
    {
        if(ParentModel is not null)
        {
            var pk = ParentModel.GetPrimaryKey();

            var fk = ModelExtensions.GetForeignKey<TModel, TChild>();

            args.Model.SetPropertyValue(fk, ParentModel.GetPropertyValue(pk));
        }
 
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

            var fkValue = ParentModel.GetPropertyValue(pk)?.ToString();

            var result = await Service.FindByParentAsync(fkValue, fk);

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
            var result = await SecureService.FindByParentEncryptedAsync(ParentModel.GetPropertyValue(pk).ToString(), fk);

            DataSource = result;
            return result;
        });
    }


}