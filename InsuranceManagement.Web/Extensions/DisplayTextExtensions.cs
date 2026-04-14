using InsuranceManagement.Web.Domain;

namespace InsuranceManagement.Web.Extensions;

public static class DisplayTextExtensions
{
    public static string ToDisplayText(this RoleType role) => role switch
    {
        RoleType.Admin => "Admin",
        RoleType.Manager => "Mudur",
        RoleType.SalesManager => "Satis Muduru",
        RoleType.Operations => "Operasyon",
        RoleType.FieldSales => "Saha Satis",
        RoleType.CallCenter => "Cagri Merkezi",
        RoleType.SystemSpecialist => "Sistem Uzmanı (Personel Harici)",
        _ => role.ToString()
    };

    public static string ToDisplayText(this AccountType type) => type switch
    {
        AccountType.Individual => "Bireysel",
        AccountType.Corporate => "Kurumsal",
        _ => type.ToString()
    };

    public static string ToDisplayText(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "-";
        }

        return value.Trim() switch
        {
            "Active" => "Aktif",
            "Inactive" => "Pasif",
            "Passive" => "Pasif",
            "Imported" => "Ice aktarildi",
            "ImportedWithWarnings" => "Uyarilarla ice aktarildi",
            "Pending" => "Beklemede",
            _ => value
        };
    }
}
