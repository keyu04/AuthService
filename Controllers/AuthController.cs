namespace AuthMicroService.Controllers;

using Asp.Versioning;
using AuthMicroService.Common.Constants;
using AuthMicroService.Common.Helpers;
using AuthMicroService.DTOs;
using AuthMicroService.Models;
using AuthMicroService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("register")]
    [EnableRateLimiting("auth-strict")]
    public async Task<ActionResult> UserRegister(UserRegister userRegister)
    {
        var result = await _authService.UserRegister(userRegister);
        return Ok(ResponseHelper.Success<string>(result, LogConst.UserRegisterSuccess));
    }

    [HttpPost("google-login")]
    public async Task<ActionResult> AuthGoogleLogin(AuthLoginModel authLoginModel)
    {
        var result = await _authService.GoogleAuthentication(authLoginModel);
        if (result == string.Empty)
            return NoContent();
        return Ok(ResponseHelper.Success<string>(result, LogConst.GoogleLoginSuccess));
    }

    [HttpPost("login")]
    [EnableRateLimiting("auth-strict")]
    public async Task<ActionResult> UserLogin(LoginCheck loginCheck)
    {
        var result = await _authService.UserLogin(loginCheck);
        return Ok(ResponseHelper.Success<string>(result, LogConst.UserLoginSuccess));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult> GetCurrentUser()
    {
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var user = await _authService.GetUserByEmail(email!);
        if (user is null)
            return NotFound();
        return Ok(ResponseHelper.Success(user, LogConst.GetUserByEmailSuccess));
    }
}