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
    public int Id { set; get; }

    [Required]
    public string? ExternalID { set; get; }
    [Required]
    public string? FirstName { set; get; }
    public string? LastName { set; get; }
    public string? ProfilePath { set; get; }
    [Required]
    public string? Email { set; get; }
    public string? Password { set; get; }

}