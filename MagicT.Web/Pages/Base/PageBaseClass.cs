using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Spreadsheet;
using Grpc.Core;
using MagicT.Client.Exceptions;
using MagicT.Web.Models;
using MagicT.Web.Pages.HelperComponents;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MagicT.Web.Pages.Base;

public abstract class PageBaseClass : ComponentBase
{
    [Inject]
    public IDialogService DialogService { get; set; }

    [Inject]
    ISnackbar Snackbar { get; set; }

    [Inject]
    public NotificationsView NotificationsView { get; set; }

 

    protected override Task OnInitializedAsync()
    {
        NotificationsView.Snackbar = Snackbar;

        NotificationsView.Notifications = new();

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
            NotificationsView.Notifications.Add(new(ex.Status.Detail, Severity.Error));

            
        }
        catch (FilterException ex)
        {
            NotificationsView.Notifications.Add(new(ex.Message, Severity.Error));

        }

        if(NotificationsView.Notifications.Any())
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
            NotificationsView.Notifications.Add(new(ex.Status.Detail, Severity.Error));
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
            NotificationsView.Notifications.Add(new(ex.Status.Detail, Severity.Error));

            NotificationsView.Fire();
        }
    }

}