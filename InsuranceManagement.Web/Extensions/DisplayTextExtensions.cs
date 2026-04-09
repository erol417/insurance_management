using InsuranceManagement.Web.Domain;

namespace InsuranceManagement.Web.Extensions;

public static class DisplayTextExtensions
{
    public static string ToDisplayText(this RoleType role) => role switch
    {
        RoleType.Admin => "Yonetici",
        RoleType.Manager => "Genel Mudur",
        RoleType.SalesManager => "Satis Muduru",
        RoleType.Operations => "Operasyon",
        RoleType.FieldSales => "Saha Personeli",
        RoleType.CallCenter => "Call Center",
        _ => role.ToString()
    };

    public static string ToDisplayText(this LeadStatus status) => status switch
    {
        LeadStatus.New => "Yeni",
        LeadStatus.Researched => "Arastirildi",
        LeadStatus.ContactFound => "Kontak Bulundu",
        LeadStatus.ReadyForAssignment => "Atamaya Hazir",
        LeadStatus.Assigned => "Atandi",
        LeadStatus.VisitScheduled => "Ziyaret Planlandi",
        LeadStatus.Visited => "Ziyaret Edildi",
        LeadStatus.ConvertedToActivity => "Aktiviteye Donustu",
        LeadStatus.Disqualified => "Elenmis",
        _ => status.ToString()
    };

    public static string ToDisplayText(this AccountType type) => type switch
    {
        AccountType.Individual => "Bireysel",
        AccountType.Corporate => "Kurumsal",
        _ => type.ToString()
    };

    public static string ToDisplayText(this ContactStatus status) => status switch
    {
        ContactStatus.Contacted => "Gorusuldu",
        ContactStatus.NotContacted => "Gorusulmedi",
        _ => status.ToString()
    };

    public static string ToDisplayText(this OutcomeStatus status) => status switch
    {
        OutcomeStatus.Positive => "Olumlu",
        OutcomeStatus.Negative => "Olumsuz",
        OutcomeStatus.Postponed => "Ertelendi",
        OutcomeStatus.SaleClosed => "Satis Oldu",
        _ => status.ToString()
    };

    public static string ToDisplayText(this ProductType type) => type switch
    {
        ProductType.Bes => "BES",
        ProductType.Life => "Hayat",
        ProductType.Health => "Saglik",
        ProductType.Travel => "Seyahat",
        ProductType.Other => "Diger",
        _ => type.ToString()
    };

    public static string ToDisplayText(this ExpenseType type) => type switch
    {
        ExpenseType.Travel => "Yol",
        ExpenseType.Meal => "Yemek",
        ExpenseType.Accommodation => "Konaklama",
        ExpenseType.Other => "Diger",
        _ => type.ToString()
    };
}
