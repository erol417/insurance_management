using InsuranceManagement.Web.Domain;
using System.Collections.Generic;

namespace InsuranceManagement.Web.Services;

public interface IExpenseService
{
    List<Expense> GetAll(int page, int pageSize, out int totalCount, string? searchTerm = null, int? employeeId = null, int? filterEmployeeId = null, string? sortBy = null, bool isDescending = false);
    Expense? GetById(int id, int? filterEmployeeId = null);
    Expense Create(Expense expense);
    Expense? Update(int id, Expense updated);
    bool Delete(int id);
    (bool isValid, Dictionary<string, string> errors) Validate(Expense expense);
}
