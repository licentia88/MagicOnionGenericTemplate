namespace MagicT.Server.Exceptions;


// Interface to define exception message retrieval
public interface IDbExceptionStrategy
{
    string GetExceptionMessage(Exception exception);
}
