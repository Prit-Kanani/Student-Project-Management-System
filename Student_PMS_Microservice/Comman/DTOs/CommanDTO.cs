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
    public string SecretKey { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int ExpiryMinutes { get; set; }
}