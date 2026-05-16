using System.ComponentModel.DataAnnotations;
namespace AuthMicroService.Models;

public class User
{
    public int Id { set; get; }


    public string? ExternalID { set; get; }
    public string? FirstName { set; get; }
    public string? LastName { set; get; }
    public string? ProfilePath { set; get; }
    public string? Provider { set; get; }
    public string? ThemePreference { set; get; }
    public string? Status { set; get; }
    public string? Email { set; get; }
    public string? Password { set; get; }

}