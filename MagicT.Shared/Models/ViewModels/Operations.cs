using MemoryPack;

namespace MagicT.Shared.Models.ViewModels;

[MemoryPackable]
public partial class Operations
{
    public int Id { get; set; }

    public string Description { get; set; }
}