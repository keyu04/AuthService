using AuthMicroService.DTOs;

namespace AuthMicroService.Services.Interfaces;

public interface IAuthService
{
    public Task<string> GoogleAuthentication(AuthLoginModel authLoginModel);
    public Task<string> UserRegister(UserRegister userRegister);
    public Task<string> UserLogin(LoginCheck loginCheck);
    public Task<UserSession> GetUserByEmail(string email);
}