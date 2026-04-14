using System.Security.Claims;
using InsuranceManagement.Web.Domain;

namespace InsuranceManagement.Web.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal principal)
        => int.Parse(principal.FindFirst("UserId")?.Value ?? "0");

    public static string GetRole(this ClaimsPrincipal principal)
        => principal.FindFirst(ClaimsIdentity.DefaultRoleClaimType)?.Value ?? "";

    public static int? GetEmployeeId(this ClaimsPrincipal principal)
    {
        var val = principal.FindFirst("EmployeeId")?.Value;
        return string.IsNullOrEmpty(val) ? null : int.Parse(val);
    }

    public static string GetFullName(this ClaimsPrincipal principal)
        => principal.FindFirst("FullName")?.Value ?? "";

    public static bool IsInRole(this ClaimsPrincipal principal, params string[] roles)
        => roles.Any(r => principal.IsInRole(r));

    // Compatibility method for existing code
    public static RoleType? GetRoleType(this ClaimsPrincipal principal)
    {
        var claim = principal.FindFirst(ClaimsIdentity.DefaultRoleClaimType)?.Value;
        return Enum.TryParse<RoleType>(claim, out var role) ? role : null;
    }
}
