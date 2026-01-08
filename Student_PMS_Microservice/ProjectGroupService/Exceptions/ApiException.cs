namespace ProjectGroupService.Exceptions;

public abstract class ApiException : Exception
{
    public int StatusCode { get; }

    protected ApiException(string message, int statusCode)
        : base(message)
    {
        StatusCode = statusCode;
    }
}


public sealed class BadRequestException : ApiException
{
    public BadRequestException(string message)
        : base(message, StatusCodes.Status400BadRequest) { }
}

public sealed class NotFoundException : ApiException
{
    public NotFoundException(string message)
        : base(message, StatusCodes.Status404NotFound) { }
}

public sealed class UnauthorizedException : ApiException
{
    public UnauthorizedException(string message)
        : base(message, StatusCodes.Status401Unauthorized) { }
}
