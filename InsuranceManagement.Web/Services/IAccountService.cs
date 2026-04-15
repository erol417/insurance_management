using InsuranceManagement.Web.Domain;
using System.Collections.Generic;

namespace InsuranceManagement.Web.Services;

public interface IAccountService
{
    List<Account> GetAll(int page, int pageSize, out int totalCount, string? searchTerm = null, string? status = null, int? employeeId = null, int? filterEmployeeId = null, string? sortBy = null, bool isDescending = false);
    Account? GetById(int id, int? filterEmployeeId = null);
    Account Create(Account account);
    Account? Update(int id, Account updated);
    bool Delete(int id);
    (bool isValid, Dictionary<string, string> errors) Validate(Account account);
    List<string> CheckDuplicate(string displayName, string? phone, string? email, string? taxNumber, int? currentId);
    List<Activity> GetPlannedVisits(int accountId);
}
