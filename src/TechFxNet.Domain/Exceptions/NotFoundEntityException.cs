namespace TechFxNet.Domain.Exceptions;

public class NotFoundEntityException : Exception
{
    public NotFoundEntityException(Exception ex) : base(ex.Message, ex.InnerException) { }

    public NotFoundEntityException(string message) : base(message) { }
}