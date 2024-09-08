using System.ComponentModel.DataAnnotations;
using Generator.Components.Args;
using Generator.Components.Interfaces;
using MagicT.Shared.Enums;
using MagicT.Shared.Extensions;
using MagicT.Shared.Services.Base;
using MagicT.Web.Shared.Extensions;
using MagicT.Web.Shared.Models;
using MagicT.Web.Shared.Pages.Audits;
using MagicT.Web.Shared.Pages.Shared;
using MessagePipe;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using ModelExtensions = MagicT.Shared.Extensions.ModelExtensions;

namespace MagicT.Web.Shared.Base;

/// <summary>
/// Abstract base class for pages that communicate with a service for CRUD operations.
/// </summary>
/// <typeparam name="TModel">The type of the model.</typeparam>
/// <typeparam name="TService">The type of the service.</typeparam>
public abstract class ServicePageBase<TModel, TService> : PageBaseClass
    where TModel : class, new()
    where TService : IMagicService<TService, TModel>
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
    /// The uploaded file.
    /// </summary>
    public IBrowserFile File { get; set; }

  

    /// <summary>
    /// The service that communicates with the backend.
    /// </summary>
    [Inject]
    protected TService Service { get; set; }

    /// <summary>
    /// The data source for the grid and view.
    /// </summary>
    protected List<TModel> DataSource { get; set; } = new();

    /// <summary>
    /// Subscriber for individual model operations.
    /// </summary>
    [Inject] public ISubscriber<Operation, TModel> Subscriber { get; set; }

    /// <summary>
    /// Initializes the component before the main initialization.
    /// </summary>
    protected override Task OnBeforeInitializeAsync()
    {
        // Subscribing to various operations
        Subscriber.Subscribe(Operation.Create, _ => InvokeAsync(StateHasChanged));
        Subscriber.Subscribe(Operation.Read, _ => InvokeAsync(StateHasChanged));
        Subscriber.Subscribe(Operation.Update, _ => InvokeAsync(StateHasChanged));
        Subscriber.Subscribe(Operation.Delete, _ => InvokeAsync(StateHasChanged));
        Subscriber.Subscribe(Operation.Stream, _ => InvokeAsync(StateHasChanged));

        return base.OnBeforeInitializeAsync();
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
        }).OnComplete(result =>
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
    /// <returns>A list of models matching the search arguments.</returns>
    protected virtual async Task<List<TModel>> ReadAsync(SearchArgs args)
    {

        //for (int i = 0; i < 10; i++)
        //{
        //    await ExecuteAsync(async () =>
        //   {
        //       var result = await Service.ReadAsync();
        //       DataSource = result;

        //       return result;
        //   });
        //}
        return await ExecuteAsync(async () =>
        {
            var result = await Service.ReadAsync();
            DataSource = result;

            return result;
        });
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

            DataSource[args.Index] = result;

            args.CurrentValue = result;

            return result;
        }).OnComplete((data, result) =>
        {
            if (result == TaskResult.Success) return Task.FromResult(data);

            // Replace the items with existing values if update fails
            // var index = DataSource.IndexByKey(args.CurrentValue);
            DataSource[args.Index] = args.OldValue;

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

    /// <summary>
    /// Handles file upload.
    /// </summary>
    /// <param name="file">The uploaded file.</param>
    public virtual void OnFileUpload(IBrowserFile file)
    {
        File = file;
    }

    /// <summary>
    /// Reads the uploaded file as a byte array.
    /// </summary>
    /// <param name="file">The uploaded file.</param>
    /// <returns>A task representing the byte array of the file.</returns>
    public virtual async Task<byte[]> ReadFileAsBytes(IBrowserFile file)
    {
        using MemoryStream memoryStream = new();

        // Copy the file stream to the memory stream
        await file.OpenReadStream().CopyToAsync(memoryStream);

        // Convert the memory stream to a byte array
        byte[] fileBytes = memoryStream.ToArray();

        return fileBytes;
    }

    public void ShowDialog<TComponent>(object model, string Title = default, DialogParameters dialogParameters =default, DialogOptions dialogOptions = default ) where TComponent:ComponentBase
    {
        if (model is not TModel) return;

        DialogService.Show<TComponent>(Title, dialogParameters, dialogOptions);
    }

    public void ShowLogs(object model)
    {
        var dialogParameters = new DialogParameters();
        var primaryKey = ModelExtensions.GetPrimaryKey<TModel>();
        var primaryKeyValue = model.GetPropertyValue(primaryKey);

        dialogParameters.Add(nameof(AuditsRecord.PrimaryKeyValue), primaryKeyValue);
        dialogParameters.Add(nameof(AuditsRecord.TableName), typeof(TModel).Name);

        var dialogOptions = new DialogOptions
        {
            CloseButton = true,
            CloseOnEscapeKey = true,
            FullScreen = true,
            Position = DialogPosition.Center
        };

        ShowDialog<AuditsRecord>(model, string.Empty, dialogParameters, dialogOptions);
    }
}

/// <summary>
/// Abstract base class for pages that communicate with a service and handle a parent-child relationship.
/// </summary>
/// <typeparam name="TModel">The type of the parent model.</typeparam>
/// <typeparam name="TChild">The type of the child model.</typeparam>
/// <typeparam name="TService">The type of the service.</typeparam>
public abstract class ServicePageBase<TModel, TChild, TService> : ServicePageBase<TChild, TService>
    where TService : IMagicService<TService, TChild>
    where TModel : new()
    where TChild : class, new()
{
    /// <summary>
    /// Gets or sets the parent model.
    /// </summary>
    [Parameter, EditorRequired]
    public TModel ParentModel { get; set; }

    /// <summary>
    /// Loads the view component.
    /// </summary>
    /// <param name="view">The view component to load.</param>
    /// <returns>A task representing the load operation.</returns>
    protected override async Task LoadAsync(IGenView<TChild> view)
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

    /// <summary>
    /// Initializes the component before the main initialization and loads child data by parent.
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
