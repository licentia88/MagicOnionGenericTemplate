using MemoryPack;

namespace MagicT.Shared.Models.ServiceModels;

[MemoryPackable]
// ReSharper disable once PartialTypeWithSinglePart
// ReSharper disable once UnusedType.Global
public partial class RESPONSE_REQUEST<TModel>
{
    public RESPONSE_REQUEST()
    {
    }

    [MemoryPackConstructor]
    public RESPONSE_REQUEST(TModel data)
    {
        Data = data;
    }

    public TModel Data { get; set; } = default!;
}