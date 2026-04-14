using InsuranceManagement.Web.Data;
using InsuranceManagement.Web.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InsuranceManagement.Web.Services;

public class EmployeeService : IEmployeeService
{
    private readonly AppDbContext _db;
    private readonly IAuditService _auditService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EmployeeService(AppDbContext db, IAuditService auditService, IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _auditService = auditService;
        _httpContextAccessor = httpContextAccessor;
    }

    public List<Employee> GetAll(string? searchTerm = null, string? sortBy = null, bool isDescending = false)
    {
        var query = _db.Employees.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var st = searchTerm.Trim().ToLower();
            query = query.Where(x => x.FullName.ToLower().Contains(st) || x.City.ToLower().Contains(st) || x.Region.ToLower().Contains(st));
        }

        if (!string.IsNullOrEmpty(sortBy))
        {
            query = sortBy.ToLower() switch
            {
                "name" => isDescending ? query.OrderByDescending(x => x.FullName) : query.OrderBy(x => x.FullName),
                "city" => isDescending ? query.OrderByDescending(x => x.City) : query.OrderBy(x => x.City),
                "region" => isDescending ? query.OrderByDescending(x => x.Region) : query.OrderBy(x => x.Region),
                _ => query.OrderBy(x => x.FullName)
            };
        }
        else
        {
            query = query.OrderBy(x => x.FullName);
        }

        return query.ToList();
    }

    public Employee? GetById(int id)
    {
        return _db.Employees
            .Include(x => x.LeadAssignments)
            .Include(x => x.Activities)
            .Include(x => x.Sales)
            .Include(x => x.Expenses)
            .FirstOrDefault(x => x.Id == id);
    }

    public Employee Create(Employee employee)
    {
        var id = (_db.Employees.Max(x => (int?)x.Id) ?? 10) + 1;
        employee.Id = id;

        _db.Employees.Add(employee);
        _auditService.Log("Employee", "Create", $"EMP-{id}", $"Yeni personel olusturuldu: {employee.FullName}.");
        _db.SaveChanges();
        return employee;
    }

    public Employee? Update(int id, Employee updated)
    {
        var employee = _db.Employees.FirstOrDefault(x => x.Id == id);
        if (employee is null) return null;

        employee.FullName = updated.FullName;
        employee.Region = updated.Region;
        employee.City = updated.City;

        _auditService.Log("Employee", "Update", $"EMP-{id}", $"Personel bilgileri guncellendi: {employee.FullName}.");
        _db.SaveChanges();
        return employee;
    }

    public bool Delete(int id)
    {
        var employee = _db.Employees.FirstOrDefault(x => x.Id == id);
        if (employee is null) return false;

        _db.Employees.Remove(employee);
        _auditService.Log("Employee", "Delete", $"EMP-{id}", $"Personel kaydi silindi: {employee.FullName}.");
        _db.SaveChanges();
        return true;
    }

    public (bool isValid, Dictionary<string, string> errors) Validate(Employee employee)
    {
        var errors = new Dictionary<string, string>();

        if (string.IsNullOrWhiteSpace(employee.FullName))
            errors.Add(nameof(employee.FullName), "Personel adi bos birakilamaz.");
        
        if (string.IsNullOrWhiteSpace(employee.Region))
            errors.Add(nameof(employee.Region), "Bolge alani zorunludur.");

        return (errors.Count == 0, errors);
    }
}
