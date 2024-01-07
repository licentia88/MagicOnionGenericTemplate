using System.Security.Authentication;
using Grpc.Core;
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

    protected override async Task OnInitializedAsync()
    {
        NotificationsView.Snackbar = Snackbar;

        NotificationsView.Notifications = new List<NotificationVM>();

        await  LoadAsync();
        await base.OnInitializedAsync();
    }

    protected virtual Task LoadAsync()
    {
        return Task.CompletedTask;
    }
    protected async Task<T> ExecuteAsync<T>(Func<Task<T>> task)
    {
        try
        {
           var result =  await task().ConfigureAwait(false);

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

    protected async Task ExecuteAsync(Func<Task> task)
    {
        try
        {
             await task().ConfigureAwait(false);

            return; ;
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