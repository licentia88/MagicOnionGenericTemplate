using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Generator.Equals;
using MagicT.Shared.Models.Base;
using MemoryPack;

namespace MagicT.Shared.Models;

[Equatable]
[MemoryPackable]
[Table(nameof(ROLES))]
// ReSharper disable once PartialTypeWithSinglePart
public sealed partial class ROLES:AUTHORIZATIONS_BASE
{
    public ROLES() => AB_AUTH_TYPE = nameof(ROLES);

 
}

[Equatable]
[MemoryPackable]
public sealed partial class FAILED_TRANSACTIONS_LOG
{
    [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int FTL_ROWID { get; set; }

    public DateTime FTL_DATE { get; set; }

    public string FTL_ERROR { get; set; }
}