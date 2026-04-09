using InsuranceManagement.Web.Domain;

namespace InsuranceManagement.Web.ViewModels;

public class NavItemVm
{
    public string Label { get; set; } = string.Empty;
    public string Controller { get; set; } = string.Empty;
    public string Action { get; set; } = "Index";
}

public class NavGroupVm
{
    public string Title { get; set; } = string.Empty;
    public List<NavItemVm> Items { get; set; } = [];
}

public class AppShellViewModel
{
    public string CurrentUserName { get; set; } = string.Empty;
    public RoleType? CurrentRole { get; set; }
    public List<NavGroupVm> Groups { get; set; } = [];
}
