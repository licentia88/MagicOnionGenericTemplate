using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagicT.Shared.Models;

[Equatable]
[MemoryPackable]
public partial class AUDIT_RECORDS_D
{
    [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ARD_ROWID { get; set; }

    public int ARD_M_REFNO { get; set; }

    public bool ARD_IS_PRIMARYKEY { get; set; } = false;

    public string ARD_PROPERTY_NAME { get; set; }

    public string ARD_OLD_VALUE { get; set; }

    public string ARD_NEW_VALUE { get; set; }
}