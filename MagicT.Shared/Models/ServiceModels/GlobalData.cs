using Generator.Equals;
using MemoryPack;

namespace MagicT.Shared.Models.ServiceModels;

[Equatable]
[MemoryPackable]
public partial class GlobalData
{
    public byte[] Shared { get; set; }
}
 