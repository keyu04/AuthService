using AuthMicroService.DTOs;

namespace AuthMicroService.Services.Interfaces;

public interface IAuthService
{
    public Task<string> googleAuthentication(AuthLoginModel authLoginModel);
    public Task<string> userRegister(UserRegister userRegister);
    public Task<string> userLogin(LoginCheck loginCheck);
}