using InsuranceManagement.Web.Domain;
using System.Collections.Generic;

namespace InsuranceManagement.Web.Services;

public interface IActivityService
{
    List<Activity> GetAll(int page, int pageSize, out int totalCount, string? searchTerm = null, int? employeeId = null, int? filterEmployeeId = null, string? sortBy = null, bool isDescending = false);
    Activity? GetById(int id, int? filterEmployeeId = null);
    Activity Create(Activity activity);
    Activity? Update(int id, Activity updated);
    bool Delete(int id);  // soft delete
    (bool isValid, Dictionary<string, string> errors) Validate(Activity activity);
}
