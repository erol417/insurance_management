using InsuranceManagement.Web.Domain;
using System.Collections.Generic;

namespace InsuranceManagement.Web.Services;

public interface ISaleService
{
    List<Sale> GetAll(int page, int pageSize, out int totalCount, string? searchTerm = null, int? employeeId = null, int? filterEmployeeId = null, string? sortBy = null, bool isDescending = false);
    Sale? GetById(int id, int? filterEmployeeId = null);
    Sale Create(Sale sale);
    Sale? Update(int id, Sale updated);
    bool Delete(int id);
    (bool isValid, Dictionary<string, string> errors) Validate(Sale sale);
}
