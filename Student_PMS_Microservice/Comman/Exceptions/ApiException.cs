namespace Comman.Exceptions;

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

public abstract class DatabaseException(
    string message,
    int? sqlErrorNumber = null,
    int statusCode = StatusCodes.Status500InternalServerError
) : ApiException(message, statusCode)
{
    public int? SqlErrorNumber { get; } = sqlErrorNumber;
}

public sealed class DuplicateKeyException(string message, int? sqlErrorNumber = null)
    : DatabaseException(message, sqlErrorNumber, StatusCodes.Status409Conflict)
{
}

public sealed class ForeignKeyViolationException(string message, int? sqlErrorNumber = null)
    : DatabaseException(message, sqlErrorNumber, StatusCodes.Status400BadRequest)
{
}

public sealed class DatabaseTimeoutException(string message, int? sqlErrorNumber = null)
    : DatabaseException(message, sqlErrorNumber, StatusCodes.Status503ServiceUnavailable)
{
}

public sealed class DatabaseServerException(string message)
    : ApiException(message, StatusCodes.Status500InternalServerError)
{
}
