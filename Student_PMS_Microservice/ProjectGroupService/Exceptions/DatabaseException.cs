namespace ProjectGroupService.Exceptions;

public abstract class DatabaseException(
    string message,
    int? sqlErrorNumber = null,
    int statusCode = StatusCodes.Status500InternalServerError
) : ApiException(message, statusCode)
{
    /// SQL Server error number .
    public int? SqlErrorNumber { get; } = sqlErrorNumber;
}

public sealed class DuplicateKeyException(string message, int? sqlErrorNumber = null) : DatabaseException(message, sqlErrorNumber, StatusCodes.Status409Conflict)
{
}

public sealed class ForeignKeyViolationException(string message, int? sqlErrorNumber = null) : DatabaseException(message, sqlErrorNumber, StatusCodes.Status400BadRequest)
{
}

public sealed class DatabaseTimeoutException(string message, int? sqlErrorNumber = null) : DatabaseException(message, sqlErrorNumber, StatusCodes.Status503ServiceUnavailable)
{
}

public sealed class DatabaseServerException(string message) : ApiException(message, StatusCodes.Status500InternalServerError)
{
}