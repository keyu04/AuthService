using AuthMicroService.DTOs;
using AuthMicroService.Models;
using AuthMicroService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthMicroService.Repositories.Implementations;

public class AuthRepositories(AppDbContext appDbContext) : IAuthRepositories
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task<string> UserLogin(string ExternalId)
    {
        try
        {
            var result = await _appDbContext.Users.FirstOrDefaultAsync(u => u.ExternalID == ExternalId);
            if (result == null)
            {
                return string.Empty;
            }
            return "result";
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task UserRegister(User user)
    {
        try
        {
            await _appDbContext.Users.AddAsync(user);
            await _appDbContext.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<User?> GetUserForLogin(string email)
    {
        try
        {
            var result = await _appDbContext.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            return result;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<UserSession> GetUserByEmail(string email)
    {
        var result = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (result is null)
        {
            return null!;
        }
        return new UserSession
        {
            Email = result.Email,
            FirstName = result.FirstName,
            LastName = result.LastName,
            Provider = result.Provider,
            ThemePreference = result.ThemePreference,
            Status = result.Status,
            ProfilePath = result.ProfilePath,
            Role = result.Role
        };
    }
}