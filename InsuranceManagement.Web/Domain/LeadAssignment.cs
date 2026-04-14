namespace InsuranceManagement.Web.Domain;

public class LeadAssignment : BaseEntity
{
    public int LeadId { get; set; }
    public Lead Lead { get; set; } = null!;
    
    public int AssignedEmployeeId { get; set; }
    public Employee AssignedEmployee { get; set; } = null!;
    
    public int AssignedByUserId { get; set; }
    public UserAccount AssignedByUser { get; set; } = null!;
    
    public DateTime AssignedAt { get; set; }
    public LeadPriority Priority { get; set; } = LeadPriority.Medium;
    public DateTime? DueDate { get; set; }
    public string? AssignmentNote { get; set; }
    public bool IsActive { get; set; } = true;
}
