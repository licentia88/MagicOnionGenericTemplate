using Generator.Equals;
using MemoryPack;

namespace MagicT.Client.Models;

[Equatable]
[MemoryPackable]
public partial class GlobalData
{
    public byte[] Shared { get; set; }
}
 