namespace InsuranceManagement.Web.Domain;

public interface ISoftDeletable
{
    DateTime? DeletedAt { get; set; }
    string? DeletedBy { get; set; }
}
