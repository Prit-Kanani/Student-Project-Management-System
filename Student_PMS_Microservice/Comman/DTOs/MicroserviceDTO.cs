namespace Comman.MicroserviceDTO;

public class CreatedAndModifiedDTO
{
    public string CreatedBy { get; set; } = string.Empty;
    public string ModifiedBy { get; set; } = string.Empty;
}

public class AuditUsersDTO
{
    public string CreatedBy { get; set; } = string.Empty;
    public string ModifiedBy { get; set; } = string.Empty;
    public string ApprovedBy { get; set; } = string.Empty;
}

public class EntityExistsDTO
{
    public int Id { get; set; }
    public bool Exists { get; set; }
}
