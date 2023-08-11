using Grpc.Core;

namespace MagicT.Client.Exceptions;

/// <summary>
/// Represents a cryption exception.
/// </summary>
public sealed class CryptionException : Exception
{
    /// <summary>
    /// Gets the status code associated with the exception.
    /// </summary>
    public StatusCode StatusCode { get; }
 
    /// <summary>
    /// Initializes a new instance of the <see cref="CryptionException"/> class.
    /// </summary>
    /// <param name="statusCode"></param>
    /// <param name="message"></param>
    public CryptionException(StatusCode statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }
}