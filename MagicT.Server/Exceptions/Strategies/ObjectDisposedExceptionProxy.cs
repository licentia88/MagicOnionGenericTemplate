namespace MagicT.Server.Exceptions.Strategies;

// Proxy class to handle ObjectDisposedException
public class ObjectDisposedExceptionStrategy : IDbExceptionStrategy
{
    public string GetExceptionMessage(Exception exception)
    {
        if (exception is ObjectDisposedException disposedEx)
            return "The object has been disposed: " + disposedEx.Message;

        return null;
    }
}