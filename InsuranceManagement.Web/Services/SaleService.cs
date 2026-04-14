using InsuranceManagement.Web.Data;
using InsuranceManagement.Web.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InsuranceManagement.Web.Services;

public class SaleService : ISaleService
{
    private readonly AppDbContext _db;
    private readonly IAuditService _auditService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SaleService(AppDbContext db, IAuditService auditService, IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _auditService = auditService;
        _httpContextAccessor = httpContextAccessor;
    }

    public List<Sale> GetAll(int page, int pageSize, out int totalCount, string? searchTerm = null, int? employeeId = null, int? filterEmployeeId = null, string? sortBy = null, bool isDescending = false)
    {
        var query = _db.Sales
            .Include(x => x.Employee)
            .Include(x => x.Account)
            .Include(x => x.InsuranceProductType)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(x => (x.Account != null && x.Account.DisplayName.Contains(searchTerm)) || x.Code.Contains(searchTerm) || (x.Account != null && x.Account.Phone != null && x.Account.Phone.Contains(searchTerm)) || (x.Account != null && x.Account.Email != null && x.Account.Email.Contains(searchTerm)) || (x.Notes != null && x.Notes.Contains(searchTerm)));
        }

        if (employeeId.HasValue)
        {
            query = query.Where(x => x.EmployeeId == employeeId.Value);
        }

        if (filterEmployeeId.HasValue)
        {
            query = query.Where(x => x.EmployeeId == filterEmployeeId.Value);
        }

        if (!string.IsNullOrEmpty(sortBy))
        {
            query = sortBy.ToLower() switch
            {
                "code" => isDescending ? query.OrderByDescending(x => x.Code) : query.OrderBy(x => x.Code),
                "date" => isDescending ? query.OrderByDescending(x => x.SaleDate) : query.OrderBy(x => x.SaleDate),
                "customer" => isDescending ? query.OrderByDescending(x => x.Account.DisplayName) : query.OrderBy(x => x.Account.DisplayName),
                "employee" => isDescending ? query.OrderByDescending(x => x.Employee.FullName) : query.OrderBy(x => x.Employee.FullName),
                "product" => isDescending ? query.OrderByDescending(x => x.InsuranceProductType.Name) : query.OrderBy(x => x.InsuranceProductType.Name),
                "amount" => isDescending ? query.OrderByDescending(x => x.SaleAmount ?? 0) : query.OrderBy(x => x.SaleAmount ?? 0),
                "count" => isDescending ? query.OrderByDescending(x => x.SaleCount) : query.OrderBy(x => x.SaleCount),
                _ => query.OrderByDescending(x => x.SaleDate)
            };
        }
        else
        {
            query = query.OrderByDescending(x => x.SaleDate);
        }

        totalCount = query.Count();
        return query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public Sale? GetById(int id, int? filterEmployeeId = null)
    {
        var query = _db.Sales
            .Include(x => x.Employee)
            .Include(x => x.Account)
            .Include(x => x.InsuranceProductType)
            .Include(x => x.Activity)
                .ThenInclude(a => a != null ? a.Lead : null)
            .AsQueryable();

        if (filterEmployeeId.HasValue)
        {
            query = query.Where(x => x.EmployeeId == filterEmployeeId.Value);
        }

        return query.FirstOrDefault(x => x.Id == id);
    }

    public Sale Create(Sale sale)
    {
        var id = (_db.Sales.IgnoreQueryFilters().Max(x => (int?)x.Id) ?? 100) + 1;
        sale.Id = id;
        sale.Code = $"SALE-{id}";

        _db.Sales.Add(sale);
        _auditService.Log("Sale", "Create", sale.Code, $"Satis olusturuldu. Musteri #{sale.AccountId}, Tutar: {sale.SaleAmount ?? 0}.");
        _db.SaveChanges();
        return sale;
    }

    public Sale? Update(int id, Sale updated)
    {
        var sale = _db.Sales.FirstOrDefault(x => x.Id == id);
        if (sale is null) return null;

        sale.SaleDate = updated.SaleDate;
        sale.EmployeeId = updated.EmployeeId;
        sale.AccountId = updated.AccountId;
        sale.ActivityId = updated.ActivityId;
        sale.ProductTypeId = updated.ProductTypeId;
        sale.CollectionAmount = updated.CollectionAmount;
        sale.ApeAmount = updated.ApeAmount;
        sale.LumpSumAmount = updated.LumpSumAmount;
        sale.MonthlyPaymentAmount = updated.MonthlyPaymentAmount;
        sale.PremiumAmount = updated.PremiumAmount;
        sale.ProductionAmount = updated.ProductionAmount;
        sale.SaleAmount = updated.SaleAmount;
        sale.SaleCount = updated.SaleCount;
        sale.Notes = updated.Notes;

        _auditService.Log("Sale", "Update", sale.Code, "Satis guncellendi.");
        _db.SaveChanges();
        return sale;
    }

    public bool Delete(int id)
    {
        var sale = _db.Sales.FirstOrDefault(x => x.Id == id);
        if (sale is null) return false;

        sale.DeletedAt = DateTime.UtcNow;
        sale.DeletedBy = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "system";

        _auditService.Log("Sale", "Delete", sale.Code, "Satis silindi (soft delete).");
        _db.SaveChanges();
        return true;
    }

    public (bool isValid, Dictionary<string, string> errors) Validate(Sale sale)
    {
        var errors = new Dictionary<string, string>();

        if (sale.EmployeeId <= 0)
            errors.Add(nameof(sale.EmployeeId), "Personel secimi zorunludur.");
        
        if (sale.AccountId <= 0)
            errors.Add(nameof(sale.AccountId), "Musteri secimi zorunludur.");

        if (sale.ProductTypeId <= 0)
            errors.Add(nameof(sale.ProductTypeId), "Urun tipi secimi zorunludur.");

        // Validation rules from former ViewModel logic
        switch (sale.ProductTypeId)
        {
            case 1: // BES
                if (sale.ApeAmount is null)
                    errors.Add(nameof(sale.ApeAmount), "BES satisinda APE tutari zorunludur.");
                if (sale.LumpSumAmount is null)
                    errors.Add(nameof(sale.LumpSumAmount), "BES satisinda Toplu Para tutari zorunludur. Degeri yoksa 0 (sifir) giriniz.");
                if (sale.MonthlyPaymentAmount is null)
                    errors.Add(nameof(sale.MonthlyPaymentAmount), "BES satisinda Aylik Odeme tutari zorunludur. Degeri yoksa 0 (sifir) giriniz.");
                if (sale.CollectionAmount is null)
                    errors.Add(nameof(sale.CollectionAmount), "BES satisinda Tahsilat tutari zorunludur.");
                break;
            case 2: // Hayat
                if (sale.PremiumAmount is null)
                    errors.Add(nameof(sale.PremiumAmount), "Hayat satisinda prim zorunludur.");
                break;
            case 3: // Saglik
                if (sale.ProductionAmount is null && sale.CollectionAmount is null)
                {
                    errors.Add(nameof(sale.ProductionAmount), "Saglik satisinda uretim veya tahsilat zorunludur.");
                    errors.Add(nameof(sale.CollectionAmount), "Saglik satisinda uretim veya tahsilat zorunludur.");
                }
                break;
            case 4: // Seyahat
                if (sale.SaleAmount is null && sale.CollectionAmount is null)
                {
                    errors.Add(nameof(sale.SaleAmount), "Seyahat satisinda satis tutari veya tahsilat zorunludur.");
                    errors.Add(nameof(sale.CollectionAmount), "Seyahat satisinda satis tutari veya tahsilat zorunludur.");
                }
                break;
            case 5: // Diger
                if (sale.SaleAmount is null)
                    errors.Add(nameof(sale.SaleAmount), "Diger urunlerde satis tutari zorunludur.");
                break;
        }

        return (errors.Count == 0, errors);
    }
}
