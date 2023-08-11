using MemoryPack;

namespace MagicT.Shared.Models.ServiceModels;

/// <summary>
/// Represents a response request with generic data.
/// </summary>
/// <typeparam name="TModel">The type of data in the response request.</typeparam>
[MemoryPackable]
// The ReSharper disable comments are usually used to suppress warnings in ReSharper.
// They may not be needed, but we keep them in the updated code for consistency.
// ReSharper disable once PartialTypeWithSinglePart
// ReSharper disable once UnusedType.Global
public sealed partial class RESPONSE_REQUEST<TModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RESPONSE_REQUEST{TModel}"/> class.
    /// </summary>
    public RESPONSE_REQUEST()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RESPONSE_REQUEST{TModel}"/> class with data.
    /// </summary>
    /// <param name="data">The data for the response request.</param>
    [MemoryPackConstructor]
    public RESPONSE_REQUEST(TModel data)
    {
        Data = data;
    }

    /// <summary>
    /// Gets or sets the data for the response request.
    /// </summary>
    public TModel Data { get; set; } = default!;
}
