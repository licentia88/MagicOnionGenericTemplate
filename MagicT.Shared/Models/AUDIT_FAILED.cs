using System.ComponentModel.DataAnnotations.Schema;
using Generator.Equals;
using MemoryPack;

namespace MagicT.Shared.Models;

[Equatable]
[MemoryPackable]
[Table(nameof(AUDIT_FAILED))]
public  partial class AUDIT_FAILED:AUDIT_BASE
{
    public string AF_ERROR { get; set; }

    public string AF_PARAMETERS { get; set; }
}