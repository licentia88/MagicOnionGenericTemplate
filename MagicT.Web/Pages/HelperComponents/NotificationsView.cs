using MagicT.Web.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MagicT.Web.Pages.HelperComponents;

// ReSharper disable once PartialTypeWithSinglePart
[RegisterScoped]
public partial class NotificationsView : ComponentBase
{
    [Inject] public ISnackbar Snackbar { get; set; }

    [Inject] public List<NotificationVM> Notifications { get; set; }
 
    protected override Task OnInitializedAsync()
    {
        Snackbar.Configuration.SnackbarVariant = Variant.Filled;
        Snackbar.Configuration.MaxDisplayedSnackbars = 10;
        Snackbar.Configuration.NewestOnTop = true;

        return base.OnInitializedAsync();
    }

    public void Fire()
    {
        if (!Notifications.Any()) return;

        foreach (var notification in Notifications)
            //var errorSeverity =
            Snackbar.Add(notification.Message, notification.Severity);

        Notifications.Clear();
    }
}