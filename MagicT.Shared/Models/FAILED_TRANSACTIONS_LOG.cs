using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Generator.Equals;
using MemoryPack;

namespace MagicT.Shared.Models;

[Equatable]
[MemoryPackable]
public sealed partial class FAILED_TRANSACTIONS_LOG
{
    [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int FTL_ROWID { get; set; }

    public DateTime FTL_DATE { get; set; }

    public string FTL_ERROR { get; set; }
}