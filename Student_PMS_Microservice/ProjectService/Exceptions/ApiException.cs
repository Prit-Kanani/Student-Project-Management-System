namespace ProjectService.Exceptions;

public abstract class ApiException(
    string message,
    int statusCode
) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}

public sealed class BadRequestException(string message) : ApiException(message, StatusCodes.Status400BadRequest)
{
}

public sealed class NotFoundException(string message) : ApiException(message, StatusCodes.Status404NotFound)
{
}

public sealed class UnauthorizedException(string message) : ApiException(message, StatusCodes.Status401Unauthorized)
{
}

public sealed class ApiServerException(string message) : ApiException(message, StatusCodes.Status500InternalServerError)
{
}