using InsuranceManagement.Web.Domain;
using InsuranceManagement.Web.ViewModels;
using System.Collections.Generic;

namespace InsuranceManagement.Web.Services;

public interface ILeadService
{
    List<Lead> GetAll(int page, int pageSize, out int totalCount, string? searchTerm = null, int? statusId = null, int? employeeId = null, int? filterEmployeeId = null, string? sortBy = null, bool isDescending = false);
    Lead? GetById(int id, int? filterEmployeeId = null);
    Lead Create(Lead lead);
    Lead? Update(int id, Lead updated);
    bool Delete(int id);
    Lead? Assign(int leadId, int employeeId, int assignedByUserId, string? note);
    Lead? StartVisit(int leadId, int userId);
    List<Lead> GetAssignments(int? employeeId, string? status);
    (bool isValid, Dictionary<string, string> errors) Validate(Lead lead);
    bool CheckDuplicate(string displayName, string? phone, int? excludeId = null);
    void AddNote(int leadId, string content, string? author = null);
    
    // Hub methods
    LeadHubViewModel? GetHubData(int leadId);
    bool ChangeStatus(int leadId, string newStatusCode, string? note = null);
}
