using InsuranceManagement.Web.Domain;
using System.Collections.Generic;

namespace InsuranceManagement.Web.Services;

public interface IEmployeeService
{
    List<Employee> GetAll(string? searchTerm = null, string? sortBy = null, bool isDescending = false);
    Employee? GetById(int id);
    Employee Create(Employee employee);
    Employee? Update(int id, Employee updated);
    bool Delete(int id);
    (bool isValid, Dictionary<string, string> errors) Validate(Employee employee);
}
