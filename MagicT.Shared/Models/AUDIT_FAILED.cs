using System.ComponentModel.DataAnnotations.Schema;
using Benutomo;

namespace MagicT.Shared.Models;

[Equatable]
[MemoryPackable]
[GenerateDataReaderMapper]
[Table(nameof(AUDIT_FAILED))]
[AutomaticDisposeImpl]
public  partial class AUDIT_FAILED:AUDIT_BASE, IDisposable, IAsyncDisposable
{
    
    public string AF_PARAMETERS { get; set; }

    public string AF_ERROR { get; set; }
}