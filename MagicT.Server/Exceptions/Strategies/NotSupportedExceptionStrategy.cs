namespace MagicT.Server.Exceptions.Strategies;

// Proxy class to handle NotSupportedException
public class NotSupportedExceptionStrategy : IDbExceptionStrategy
{
    public string GetExceptionMessage(Exception exception)
    {
        if (exception is NotSupportedException notSupportedEx)
        {
            return "The requested operation is not supported: " + notSupportedEx.Message;
        }

        return null;
    }
}
