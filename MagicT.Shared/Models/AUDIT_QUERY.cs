using System.ComponentModel.DataAnnotations.Schema;

namespace MagicT.Shared.Models;

[Equatable]
[MemoryPackable]
[GenerateDataReaderMapper]
[Table(nameof(AUDIT_QUERY))]
public partial class AUDIT_QUERY: AUDIT_BASE
{    
    public string AQ_PARAMETERS { get; set; }
}