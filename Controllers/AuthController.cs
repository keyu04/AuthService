namespace AuthMicroService.Controllers;

using AuthMicroService.DTOs;
using AuthMicroService.Models;
using AuthMicroService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("AuthService")]
public class AuthController(IAuthService AuthMicroService) : ControllerBase
{
    private readonly IAuthService _AuthMicroService = AuthMicroService;

    [HttpPost("UserRegister")]
    public async Task<ActionResult> UserRegister(UserRegister userRegister)
    {
        try
        {
            var result = await _AuthMicroService.userRegister(userRegister);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message); // ← now you will SEE the real error
        }
    }

    [HttpPost("GoogleLogin")]
    public async Task<ActionResult> AuthGoogleLogin(AuthLoginModel authLoginModel)
    {
        try
        {
            var result = await _AuthMicroService.googleAuthentication(authLoginModel);
            if (result == string.Empty)
                return NoContent();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("UserLogin")]
    public async Task<ActionResult> UserLogin(LoginCheck loginCheck)
    {
        try
        {
            var result = await _AuthMicroService.userLogin(loginCheck);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}