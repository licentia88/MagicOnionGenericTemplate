using System;
using Grpc.Core;

namespace MagicT.Client.Exceptions;

public class FilterException : Exception
{
    public StatusCode StatusCode { get; }

    public FilterException(string message, StatusCode statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}

