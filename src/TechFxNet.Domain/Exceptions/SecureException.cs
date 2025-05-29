namespace TechFxNet.Domain.Exceptions;

public class SecureException : Exception
{
    public SecureException(Exception ex) : base(ex.Message, ex.InnerException) { }

    public SecureException(string message) : base(message) { }
}