using System.ComponentModel.DataAnnotations;

namespace AuthMicroService.DTOs;

public class AuthLoginModel
{
    public string? Token { set; get; }
    public string? Provider { get; set; }

}

public class LoginCheck
{
    public string? Email { set; get; }
    public string? Password { set; get; }
}

public class UserRegister
{

    public string? FirstName { set; get; }
    public string? LastName { set; get; }
    public string? Provider { set; get; } = string.Empty;
    public string? ThemePreference { set; get; } = "Light";
    public string? Status { set; get; } = "Active";
    public string? ProfilePath { set; get; } = string.Empty;
    [Required]
    public string? Email { set; get; }
    [Required]
    public string? Password { set; get; }
}

public class UserSession
{
    public string? Email { set; get; }
    public string? FirstName { set; get; }
    public string? LastName { set; get; }
    public string? Provider { set; get; }
    public string? ThemePreference { set; get; }
    public string? Status { set; get; }
    public string? ProfilePath { set; get; }
}