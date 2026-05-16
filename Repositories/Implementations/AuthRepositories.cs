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

    public async Task<string> LoginCheck(User user)
    {
        try
        {
            var result = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Email == user.Email && u.Password == user.Password);
            if (result == null)
            {
                return string.Empty;
            }
            return null!;
        }
        catch (Exception)
        {
            throw;
        }
    }
}