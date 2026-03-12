namespace Comman.DTOs.CommanDTOs;

public class OperationResultDTO
{
    public int Id { get; set; }
    public int RowsAffected { get; set; }
}
public class ListResult<T> where T : class
{
    public int TotalCount { get; set; }
    public List<T> Items { get; set; } = new List<T>();
}

public class OptionDTO
{
    public int Id { get; set; }
    public required string Name { get; set; }
}
public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; }
}
