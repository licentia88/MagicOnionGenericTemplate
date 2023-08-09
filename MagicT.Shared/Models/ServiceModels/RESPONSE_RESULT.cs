using MemoryPack;

namespace MagicT.Shared.Models.ServiceModels;
/// <summary>
/// Represents a response result with generic data.
/// </summary>
/// <typeparam name="TModel">The type of data in the response result.</typeparam>
[MemoryPackable]
// The ReSharper disable comments are usually used to suppress warnings in ReSharper.
// They may not be needed, but we keep them in the updated code for consistency.
// ReSharper disable once InconsistentNaming
// ReSharper disable once PartialTypeWithSinglePart
public partial class RESPONSE_RESULT<TModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RESPONSE_RESULT{TModel}"/> class.
    /// </summary>
    public RESPONSE_RESULT()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RESPONSE_RESULT{TModel}"/> class with data.
    /// </summary>
    /// <param name="data">The data for the response result.</param>
    [MemoryPackConstructor]
    public RESPONSE_RESULT(TModel data)
    {
        Data = data;
    }

    /// <summary>
    /// Gets or sets the data for the response result.
    /// </summary>
    public TModel Data { get; set; } = default!;
}
