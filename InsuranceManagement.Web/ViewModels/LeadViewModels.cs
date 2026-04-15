using InsuranceManagement.Web.Domain;
using System;
using System.Collections.Generic;

namespace InsuranceManagement.Web.ViewModels;

public class LeadHubViewModel
{
    // Lead temel bilgileri
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? Note { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string StatusCode { get; set; } = string.Empty;
    public string SourceName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string ContactName { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public int? ConvertedActivityId { get; set; }

    // Notlar
    public List<LeadNoteInfo> Notes { get; set; } = new();

    // Atama bilgisi
    public LeadAssignmentInfo? CurrentAssignment { get; set; }
    public List<LeadAssignmentInfo> AssignmentHistory { get; set; } = new();

    // Bağlı müşteri
    public LinkedAccountInfo? LinkedAccount { get; set; }

    // Aktivite geçmişi
    public List<LeadActivityInfo> Activities { get; set; } = new();

    // Satışlar
    public List<LeadSaleInfo> Sales { get; set; } = new();

    // Durum aksiyonları
    public List<string> AvailableActions { get; set; } = new();

    // Atama için personel listesi
    public List<Employee> AvailableEmployees { get; set; } = new();

    // Seçili personelin gelecek ziyaretleri (çakışma kontrolü için)
    public List<EmployeePlannedVisitInfo> EmployeeUpcomingVisits { get; set; } = new();
}

public class EmployeePlannedVisitInfo
{
    public DateTime PlannedDate { get; set; }
    public string LeadName { get; set; } = string.Empty;
    public int? DurationMinutes { get; set; }
}


public class LeadAssignmentInfo
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string AssignedByName { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; }
    public string? Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Note { get; set; }
    public bool IsActive { get; set; }
}

public class LinkedAccountInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? AccountType { get; set; }
    public string? City { get; set; }
    public string? Phone { get; set; }
}

public class LeadActivityInfo
{
    public int Id { get; set; }
    public DateTime ActivityDate { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string ContactStatus { get; set; } = string.Empty;
    public string ContactStatusCode { get; set; } = string.Empty;
    public string OutcomeStatus { get; set; } = string.Empty;
    public string? Summary { get; set; }
}

public class LeadSaleInfo
{
    public int Id { get; set; }
    public DateTime SaleDate { get; set; }
    public string ProductType { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public decimal? Amount { get; set; }
}

public class LeadNoteInfo
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsSystemNote { get; set; }
}
