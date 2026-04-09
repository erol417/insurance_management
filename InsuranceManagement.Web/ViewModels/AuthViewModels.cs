using System.ComponentModel.DataAnnotations;

namespace InsuranceManagement.Web.ViewModels;

public class LoginViewModel
{
    [Required]
    [Display(Name = "Kullanici adi")]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Sifre")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
