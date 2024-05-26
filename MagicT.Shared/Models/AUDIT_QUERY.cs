using System.ComponentModel.DataAnnotations.Schema;
using Generator.Equals;
using MemoryPack;
using MapDataReader;

namespace MagicT.Shared.Models;

[Equatable]
[MemoryPackable]
[GenerateDataReaderMapper]
[Table(nameof(AUDIT_QUERY))]
public partial class AUDIT_QUERY: AUDIT_BASE
{    
    public string AQ_PARAMETERS { get; set; }
}