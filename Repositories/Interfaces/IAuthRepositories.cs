using AuthMicroService.DTOs;
using AuthMicroService.Models;

namespace AuthMicroService.Repositories.Interfaces;

public interface IAuthRepositories
{
    public Task<string> UserLogin(string ExternalId);
    public Task UserRegister(User user);
    public Task<User?> GetUserForLogin(string email);
    public Task<UserSession> GetUserByEmail(string email);
}