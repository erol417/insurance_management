namespace InsuranceManagement.Web.Domain;

public enum RoleType
{
    Admin,
    Manager,
    SalesManager,
    Operations,
    FieldSales,
    CallCenter,
    SystemSpecialist
}

public enum AccountType
{
    Individual,
    Corporate
}

public enum LeadPriority
{
    Low,
    Medium,
    High,
    Urgent
}
public enum LeadStatus
{
    New = 1,
    ReadyForAssignment = 2,
    Assigned = 3,
    Visited = 4,
    Converted = 5,
    Disqualified = 9
}
