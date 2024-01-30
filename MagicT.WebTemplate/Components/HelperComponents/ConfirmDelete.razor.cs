using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MagicT.WebTemplate.Components.HelperComponents;

public partial class ConfirmDelete
{
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

    private void Close(DialogResult dialogResult)
    {
        MudDialog.Close(dialogResult);
    }


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