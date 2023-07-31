using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MagicT.Web.Pages.HelperComponents;

public partial class ConfirmDelete
{
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }

    void Close(DialogResult dialogResult) => MudDialog.Close(dialogResult);



    private void Cancel()
    {
        // Handle cancel button click
        Close(DialogResult.Cancel());
    }

    private void Confirm()
    {
        // Handle confirm button click
        // Perform delete operation here
        Close(DialogResult.Ok(true));
    }
}

