using System.Security.Claims;
using InsuranceManagement.Web.Domain;

namespace InsuranceManagement.Web.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int? GetUserId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claim, out var id) ? id : null;
    }

    public static int? GetEmployeeId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirstValue("employeeId");
        return int.TryParse(claim, out var id) ? id : null;
    }

    public static RoleType? GetRoleType(this ClaimsPrincipal user)
    {
        var claim = user.FindFirstValue(ClaimTypes.Role);
        return Enum.TryParse<RoleType>(claim, out var role) ? role : null;
    }
}
