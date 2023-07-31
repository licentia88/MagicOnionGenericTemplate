using MemoryPack;

namespace MagicT.Shared.Models.ServiceModels;

[MemoryPackable]
public partial class RESPONSE_RESULT<TModel>
{
    public TModel Data { get; set; } = default!;

    public RESPONSE_RESULT()
    {

    }

    [MemoryPackConstructor]
    public RESPONSE_RESULT(TModel data)
    {
        Data = data;
    }
}