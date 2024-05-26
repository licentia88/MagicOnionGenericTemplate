using System.ComponentModel.DataAnnotations;
using Generator.Components.Args;
using Generator.Components.Interfaces;
using MagicT.Shared.Enums;
using MagicT.Shared.Extensions;
using MagicT.Shared.Services.Base;
using MagicT.Web.Shared.Extensions;
using MagicT.Web.Shared.HelperComponents;
using MagicT.Web.Shared.Models;
using MessagePipe;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace MagicT.Web.Shared.Base;

public abstract class ServicePageBase<TModel, TService> : PageBaseClass
    where TModel : class, new()
    where TService : IMagicService<TService, TModel>
{
    public IGenGrid<TModel> Grid;

    protected IGenView<TModel> View;

    public IBrowserFile File { get; set; }

    [CascadingParameter(Name = nameof(PublicKey))]
    protected byte[] PublicKey { get; set; }

    [Inject]
    protected  TService Service { get; set; }

    //[Inject]
    protected List<TModel> DataSource { get; set; } = new();

    [Inject] public ISubscriber<Operation, TModel> Subscriber { get; set; }



    protected override Task OnBeforeInitializeAsync()
    {
        Subscriber.Subscribe(Operation.Create, _ => InvokeAsync(StateHasChanged));
        Subscriber.Subscribe(Operation.Read, _ => InvokeAsync(StateHasChanged));
        Subscriber.Subscribe(Operation.Update, _ => InvokeAsync(StateHasChanged));
        Subscriber.Subscribe(Operation.Delete, _ => InvokeAsync(StateHasChanged));
        Subscriber.Subscribe(Operation.Stream, _ => InvokeAsync(StateHasChanged));

        return base.OnBeforeInitializeAsync();
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



    protected virtual async Task<List<TModel>> ReadAsync(SearchArgs args)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await Service.ReadAsync();

            DataSource = result;

            return result;
        });
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


    public virtual void OnFileUpload(IBrowserFile file)
    {
        File = file;
    }

    public virtual async Task<byte[]> ReadFileAsBytes(IBrowserFile file)
    {
        using MemoryStream memoryStream = new();

        // Copy the file stream to the memory stream
        await file.OpenReadStream().CopyToAsync(memoryStream);

        // Convert the memory stream to a byte array
        byte[] fileBytes = memoryStream.ToArray();

        return fileBytes;
    }
}

public abstract class ServicePageBase<TModel, TChild, TService> : ServicePageBase<TChild, TService>
    where TService : IMagicService<TService, TChild>
    where TModel : new()
    where TChild : class, new()
{
    [Parameter, EditorRequired]
    public TModel ParentModel { get; set; }

    protected override  async Task LoadAsync(IGenView<TChild> view)
    {
        await base.LoadAsync(view);
        
        var results = new List<ValidationResult>();
 
        var validationContext = new ValidationContext(ParentModel);

        bool isValid = Validator.TryValidateObject(ParentModel, validationContext, results, true);

        if (!isValid)
        {
            NotificationsView.Notifications.Add(new NotificationVM("Parent is not valid", Severity.Error));
            
            NotificationsView.Fire();

            View.ShouldShowDialog = false;
        }
        
    }

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