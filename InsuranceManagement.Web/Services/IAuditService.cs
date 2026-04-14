namespace InsuranceManagement.Web.Services;

public interface IAuditService
{
    void Log(string module, string actionType, string entityCode, string detail);
}
