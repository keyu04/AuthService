using Google.Apis.Auth;
using AuthMicroService.Services.Interfaces;
using AuthMicroService.Common.Settings;
using AuthMicroService.Repositories.Interfaces;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using AuthMicroService.DTOs;
using AuthMicroService.Models;
namespace AuthMicroService.Services.Implementations;

public class AuthService(IAuthRepositories authRepositories) : IAuthService
{
    public readonly IAuthRepositories _authRepositories = authRepositories;
    public async Task<string> GoogleAuthentication(AuthLoginModel authLoginModel)
    {
        try
        {
            var payload = await verifyGoogleToken(authLoginModel.Token!);

            if (payload == null)
            {
                return string.Empty;
            }

            var result = await _authRepositories.UserLogin(payload.Subject);
            if (result == string.Empty)
            {
                var user = new User()
                {
                    ExternalID = payload.Subject,
                    FirstName = payload.GivenName,
                    LastName = payload.FamilyName,
                    Email = payload.Email,
                    ProfilePath = payload.Picture,
                    Provider = "Google",
                    ThemePreference = "Light",
                    Status = "Active"
                };
                await _authRepositories.UserRegister(user);
            }

            var token = TokenGenerator(payload, null!);
            return token;
        }
        catch (Exception ex)
        {
            throw new Exception("googleAuthentication Fail" + ex.Message);
        }
    }
    public async Task<GoogleJsonWebSignature.Payload> verifyGoogleToken(string token)
    {
        var setting = new GoogleJsonWebSignature.ValidationSettings()
        {
            Audience = new[] { GoogleSetting.ClientId }
        };

        var payload = await GoogleJsonWebSignature.ValidateAsync(token, setting);
        return payload;
    }

    public string TokenGenerator(GoogleJsonWebSignature.Payload? payload, UserSession? user)
    {
        var claims = new[]
        {
            new Claim("SubjectId", payload?.Subject ?? string.Empty),
            new Claim(ClaimTypes.Name, user?.FirstName ?? payload?.GivenName ?? string.Empty),
            new Claim(ClaimTypes.Surname, user?.LastName ?? payload?.FamilyName ?? string.Empty),
            new Claim(ClaimTypes.Email, user?.Email ?? payload?.Email ?? string.Empty),
            new Claim("Provider", user?.Provider ?? string.Empty),
            new Claim("ThemePreference", user?.ThemePreference ?? string.Empty),
            new Claim("Status", user?.Status ?? string.Empty),
            new Claim("ProfilePath", user?.ProfilePath ?? payload?.Picture ?? string.Empty)
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Settings.JwtSecreteKey!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: Settings.JwtIssuer,
            audience: Settings.JwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<string> UserRegister(UserRegister userRegister)
    {
        try
        {
            var user = new User()
            {
                FirstName = userRegister.FirstName,
                LastName = userRegister.LastName,
                Status = userRegister.Status,
                ThemePreference = userRegister.ThemePreference,
                Provider = userRegister.Provider,
                ProfilePath = userRegister.ProfilePath,
                Email = userRegister.Email,
                Password = userRegister.Password
            };
            await _authRepositories.UserRegister(user);
            return "User Registered Successfully";
        }
        catch (Exception ex)
        {
            throw new Exception("User Registration Fail" + ex);
        }
    }

    public async Task<string> UserLogin(LoginCheck loginCheck)
    {
        try
        {
            var user = new User()
            {
                Email = loginCheck.Email,
                Password = loginCheck.Password
            };

            var result = await _authRepositories.LoginCheck(user);
            var token = TokenGenerator(null, result);
            return token;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<UserSession> GetUserByEmail(string email)
    {
        try
        {
            var result = await _authRepositories.GetUserByEmail(email);
            return result;
        }
        catch (Exception)
        {
            throw;
        }
    }

}
