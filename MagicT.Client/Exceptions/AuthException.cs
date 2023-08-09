using Grpc.Core;

namespace MagicT.Client.Exceptions;

/// <summary>
/// Represents an authentication exception.
/// </summary>
public class AuthException : Exception
{
    /// <summary>
    /// Gets the status code associated with the exception.
    /// </summary>
    public StatusCode StatusCode { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthException"/> class.
    /// </summary>
    /// <param name="statusCode">The status code of the exception.</param>
    /// <param name="message">The exception message.</param>
    public AuthException(StatusCode statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }
}
