using System.Security.Authentication;
using Grpc.Core;
using MagicT.Web.Shared.Models;
using MagicT.WebTemplate.Components.HelperComponents;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace MagicT.Web.Shared.Base;

/// <summary>
/// An abstract base class for pages in the application.
/// </summary>
public abstract class PageBaseClass : ComponentBase
{
    /// <summary>
    /// Gets or sets the dialog service for displaying dialogs.
    /// </summary>
    [Inject]
    public IDialogService DialogService { get; set; }

    /// <summary>
    /// Gets or sets the MudDialog instance for cascading parameters.
    /// </summary>
#pragma warning disable IDE0051 // Remove unused private members
    [CascadingParameter]
    MudDialogInstance MudDialog { get; set; }
#pragma warning restore IDE0051 // Remove unused private members

    /// <summary>
    /// Gets or sets the snackbar service for displaying notifications.
    /// </summary>
    [Inject] private ISnackbar Snackbar { get; set; }

    /// <summary>
    /// Gets or sets the notifications view component.
    /// </summary>
    [Inject] public NotificationsView NotificationsView { get; set; }

    /// <summary>
    /// Gets or sets the navigation manager for handling navigation.
    /// </summary>
    [Inject] public NavigationManager NavigationManager { get; set; }

    /// <summary>
    /// Overrides the OnInitializedAsync method to initialize the component.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override async Task OnInitializedAsync()
    {
        NotificationsView.Snackbar = Snackbar;
        NotificationsView.Notifications = new List<NotificationVM>();
        await OnBeforeInitializeAsync();
        await base.OnInitializedAsync();
    }

    /// <summary>
    /// A virtual method that can be overridden to perform additional initialization tasks before the component is initialized.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual Task OnBeforeInitializeAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Executes an asynchronous task and handles any exceptions that may occur.
    /// </summary>
    /// <typeparam name="T">The type of the result returned by the task.</typeparam>
    /// <param name="task">The task to be executed.</param>
    /// <returns>The result of the task, or the default value if an exception occurs.</returns>
    protected async Task<T> ExecuteAsync<T>(Func<Task<T>> task)
    {
        try
        {
            var result = await task().ConfigureAwait(false);
            return result;
        }
        catch (RpcException ex)
        {
            NotificationsView.Notifications.Add(new NotificationVM(ex.Status.Detail, Severity.Error));
            //throw new OnCompleteException();
        }
        catch (AuthenticationException ex)
        {
            NotificationsView.Notifications.Add(new NotificationVM(ex.Message, Severity.Error));
        }
        catch (JSDisconnectedException ex)
        {
            //Ignore
        }

        if (NotificationsView.Notifications.Any())
            NotificationsView.Fire();

        return default;
    }

    /// <summary>
    /// Executes an asynchronous task and handles any exceptions that may occur.
    /// </summary>
    /// <param name="task">The task to be executed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected async Task ExecuteAsync(Func<Task> task)
    {
        try
        {
            await task().ConfigureAwait(false);
        }
        catch (RpcException ex)
        {
            NotificationsView.Notifications.Add(new NotificationVM(ex.Status.Detail, Severity.Error));
            //throw new OnCompleteException();
        }
        catch (AuthenticationException ex)
        {
            NotificationsView.Notifications.Add(new NotificationVM(ex.Message, Severity.Error));
        }
        catch (JSDisconnectedException ex)
        {
            //Ignore
        }

        if (NotificationsView.Notifications.Any())
            NotificationsView.Fire();
    }

    /// <summary>
    /// Executes a synchronous task and handles any exceptions that may occur.
    /// </summary>
    /// <typeparam name="T">The type of the result returned by the task.</typeparam>
    /// <param name="task">The task to be executed.</param>
    protected void Execute<T>(Func<T> task)
    {
        try
        {
            task();
        }
        catch (RpcException ex)
        {
            NotificationsView.Notifications.Add(new NotificationVM(ex.Status.Detail, Severity.Error));
        }
        catch (AuthenticationException ex)
        {
            NotificationsView.Notifications.Add(new NotificationVM(ex.Message, Severity.Error));
        }
        catch (JSDisconnectedException ex)
        {
            //Ignore
        }

        if (NotificationsView.Notifications.Any())
            NotificationsView.Fire();
    }
}