namespace AppCollect.Domain.Exceptions;

public class AppCollectDomainException : Exception
{
    public AppCollectDomainException()
    {
    }

    public AppCollectDomainException(string message) : base(message)
    {
    }

    public AppCollectDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}