namespace InsuranceManagement.Web.Domain;

public enum RoleType
{
    Admin,
    Manager,
    SalesManager,
    Operations,
    FieldSales,
    CallCenter
}

public enum LeadStatus
{
    New,
    Researched,
    ContactFound,
    ReadyForAssignment,
    Assigned,
    VisitScheduled,
    Visited,
    ConvertedToActivity,
    Disqualified
}

public enum AccountType
{
    Individual,
    Corporate
}

public enum ContactStatus
{
    Contacted,
    NotContacted
}

public enum OutcomeStatus
{
    Positive,
    Negative,
    Postponed,
    SaleClosed
}

public enum ProductType
{
    Bes,
    Life,
    Health,
    Travel,
    Other
}

public enum ExpenseType
{
    Travel,
    Meal,
    Accommodation,
    Other
}
