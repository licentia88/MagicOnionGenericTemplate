using MagicT.Shared.Models;
using MagicT.Shared.Models.ViewModels;
using Microsoft.AspNetCore.Components;

namespace MagicT.Web.Shared.Pages.Audits;

public partial class AuditsRecord
{
    [Inject]
    public Lazy<List<USERS>> UsersList { get; set; }

    [Inject]
    public Lazy<List<Operations>> OperationsList { get; set; }

    [Parameter]
    public object PrimaryKeyValue { get; set; }

    [Parameter]
    public string TableName { get; set; }

    private bool IsSingleRecord => PrimaryKeyValue != null && !string.IsNullOrEmpty(TableName);

    protected override async Task OnBeforeInitializeAsync()
    {
        await base.OnBeforeInitializeAsync();

        if (IsSingleRecord)
        {
            await ExecuteAsync(async () =>
            {
                DataSource = await Service.GetRecordLogsAsync(TableName, PrimaryKeyValue.ToString());

            });
        }
    }
}

