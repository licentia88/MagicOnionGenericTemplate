using Grpc.Core;
using MagicT.Client.Exceptions;
using MagicT.Web.Models;
using MagicT.Web.Pages.HelperComponents;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace MagicT.Web.Pages.Base;

public abstract class PageBaseClass : ComponentBase
{
    [Inject]
    public IDialogService DialogService { get; set; }

#pragma warning disable IDE0051 // Remove unused private members
    [CascadingParameter]
    MudDialogInstance MudDialog { get; set; }
#pragma warning restore IDE0051 // Remove unused private members

    [Inject] private ISnackbar Snackbar { get; set; }

    [Inject] public NotificationsView NotificationsView { get; set; }


    [Inject] public NavigationManager NavigationManager { get; set; }

    protected override Task OnInitializedAsync()
    {
        NotificationsView.Snackbar = Snackbar;

        NotificationsView.Notifications = new List<NotificationVM>();

        return base.OnInitializedAsync();
    }

    protected async Task<T> ExecuteAsync<T>(Func<Task<T>> task)
    {
#pragma warning disable IDE0059 // Unnecessary assignment of a value
        try
        {
           var result =  await task().ConfigureAwait(false);

            return result;
        }
        catch (RpcException ex)
        {
            NotificationsView.Notifications.Add(new NotificationVM(ex.Status.Detail, Severity.Error));
        }
        catch (FilterException ex)
        {
            NotificationsView.Notifications.Add(new NotificationVM(ex.Message, Severity.Error));
        }
        catch (AuthException ex)
        {
            //Occurs when token decoded in server and deemed expired 
            if (ex.StatusCode == StatusCode.ResourceExhausted)
            {
            }

            //When token is empty for somereasone
            if (ex.StatusCode == StatusCode.NotFound)
            {
            }
        }
        catch (JSDisconnectedException ex)
        {
            //Ignore
        }
        catch (Exception ex)
        {
        }
#pragma warning restore IDE0059 // Unnecessary assignment of a value

        if (NotificationsView.Notifications.Any())
            NotificationsView.Fire();

        return default;
    }

    protected async Task ExecuteAsync(Func<Task> task)
    {
#pragma warning disable IDE0059 // Unnecessary assignment of a value
        try
        {
            await task().ConfigureAwait(false);
        }
        catch (RpcException ex)
        {
            NotificationsView.Notifications.Add(new NotificationVM(ex.Status.Detail, Severity.Error));
            NotificationsView.Fire();
        }
        catch (JSDisconnectedException ex)
        {
            //Ignore
        }
#pragma warning restore IDE0059 // Unnecessary assignment of a value
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value",
        Justification = "<Pending>")]
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
        catch (FilterException ex)
        {
            NotificationsView.Notifications.Add(new NotificationVM(ex.Message, Severity.Error));
        }
        catch (AuthException ex)
        {
            //When token is empty for somereasone
            if (ex.StatusCode == StatusCode.NotFound)
            {
            }
        }
        catch (JSDisconnectedException ex)
        {
            //Ignore
        }

        if (NotificationsView.Notifications.Any())
            NotificationsView.Fire();
    }
}