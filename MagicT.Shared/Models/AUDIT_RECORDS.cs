using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Shared.Models;

[Equatable]
[MemoryPackable]
[Index( nameof(AR_TABLE_NAME), nameof(AR_PROPERTY_NAME))]
[GenerateDataReaderMapper]
[Table(nameof(AUDIT_RECORDS))]
public partial class AUDIT_RECORDS:AUDIT_BASE
{
    public string AR_TABLE_NAME { get; set; }

    public bool AR_IS_PRIMARYKEY { get; set; }

    public string AR_PROPERTY_NAME { get; set; }

    public string AR_OLD_VALUE { get; set; }

    public string AR_NEW_VALUE { get; set; }

}
