using System.ComponentModel.DataAnnotations.Schema;
using Benutomo;

namespace MagicT.Shared.Models;

[Equatable]
[MemoryPackable]
[GenerateDataReaderMapper]
[AutomaticDisposeImpl]
[Table(nameof(AUDIT_QUERY))]
public partial class AUDIT_QUERY: AUDIT_BASE,IDisposable
{    
    public string AQ_PARAMETERS { get; set; }
}