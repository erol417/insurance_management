using InsuranceManagement.Web.Data;
using InsuranceManagement.Web.Domain;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace InsuranceManagement.Web.Services;

public class AuditService : IAuditService
{
    private readonly AppDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditService(AppDbContext db, IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
    }

    public void Log(string module, string actionType, string entityCode, string detail)
    {
        var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "system";
        
        var maxDbId = _db.AuditLogs.Max(x => (int?)x.Id) ?? 0;
        var maxLocalId = _db.AuditLogs.Local.Max(x => (int?)x.Id) ?? 0;
        var nextId = Math.Max(maxDbId, maxLocalId) + 1;

        _db.AuditLogs.Add(new AuditLog
        {
            Id = nextId,
            CreatedAt = DateTime.UtcNow, // Db Context handles local times
            UserName = userName,
            Module = module,
            ActionType = actionType,
            EntityCode = entityCode,
            Detail = detail
        });
        
        // Note: the caller is responsible for Db.SaveChanges()
    }
}
