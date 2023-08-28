using System.Runtime.InteropServices;
using Grpc.Core;
using MagicT.Client.Exceptions;
using MagicT.Web.Models;
using MagicT.Web.Pages.HelperComponents;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MagicT.Web.Pages.Base;

public abstract class PageBaseClass : ComponentBase
{
    [Inject] public IDialogService DialogService { get; set; }

    [Inject] private ISnackbar Snackbar { get; set; }

    [Inject] public NotificationsView NotificationsView { get; set; }


    [Inject]
    public NavigationManager NavigationManager { get; set; }

    protected override Task OnInitializedAsync()
    {
        NotificationsView.Snackbar = Snackbar;

        NotificationsView.Notifications = new List<NotificationVM>();

        return base.OnInitializedAsync();
    }
 
    protected async Task ExecuteAsync<T>(Func<Task<T>> task)
    {
        try
        {
            await task().ConfigureAwait(false);
        }
        catch (RpcException ex)
        {
            NotificationsView.Notifications.Add(new NotificationVM(ex.Status.Detail, Severity.Error));
        }
        catch (FilterException ex)
        {
            NotificationsView.Notifications.Add(new NotificationVM(ex.Message, Severity.Error));
        }
        catch(AuthException ex)
        {
            //Occurs when token decoded in server and deemed expired 
            if(ex.StatusCode == StatusCode.ResourceExhausted)
            {

            }

            //When token is empty for somereasone
            if(ex.StatusCode == StatusCode.NotFound)
            {

            }
        }

        if (NotificationsView.Notifications.Any())
            NotificationsView.Fire();
    }

    protected async Task ExecuteAsync(Func<Task> task)
    {
        try
        {
            await task().ConfigureAwait(false);
        }
        catch (RpcException ex)
        {
            NotificationsView.Notifications.Add(new NotificationVM(ex.Status.Detail, Severity.Error));
            NotificationsView.Fire();
        }
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

            NotificationsView.Fire();
        }
    }
}