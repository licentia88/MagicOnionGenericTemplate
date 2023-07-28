namespace Magic.Server.Exceptions.Strategies;

// Proxy class to handle InvalidOperationException
public class InvalidOperationExceptionStrategy : IDbExceptionStrategy
{
    public string GetExceptionMessage(Exception exception)
    {
        if (exception is InvalidOperationException invalidOperationEx)
        {
            return "Invalid operation: " + invalidOperationEx.Message;
        }

        return null;
    }
}
