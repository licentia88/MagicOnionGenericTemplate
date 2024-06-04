using System.ComponentModel.DataAnnotations.Schema;

namespace MagicT.Shared.Models;

[Equatable]
[MemoryPackable]
[GenerateDataReaderMapper]
[Table(nameof(AUDIT_FAILED))]
public  partial class AUDIT_FAILED:AUDIT_BASE
{
    
    public string AF_PARAMETERS { get; set; }

    public string AF_ERROR { get; set; }
}