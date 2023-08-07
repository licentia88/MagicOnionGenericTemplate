using Grpc.Core;

namespace MagicT.Client.Exceptions;

public class AuthException : Exception
{
    public StatusCode StatusCode { get; }

    public AuthException(StatusCode statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }
}