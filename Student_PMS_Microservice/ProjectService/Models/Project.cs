using Comman.Functions;
using System.ComponentModel.DataAnnotations;

namespace ProjectService.Models;

public class Project
{
    [Key]
    [OptionId]
    public int ProjectID { get; set; }

    [OptionName]
    public string ProjectName { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public bool? IsApproved { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime? Modified { get; set; }
    public int CreatedByID { get; set; }
    public int? ModifiedByID { get; set; }
}
