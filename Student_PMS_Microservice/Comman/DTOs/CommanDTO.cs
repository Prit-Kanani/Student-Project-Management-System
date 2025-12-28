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