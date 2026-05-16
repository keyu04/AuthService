namespace AuthMicroService.Controllers;

using AuthMicroService.Common.Constants;
using AuthMicroService.Common.Helpers;
using AuthMicroService.DTOs;
using AuthMicroService.Models;
using AuthMicroService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
            var result = await _AuthMicroService.UserRegister(userRegister);
            return Ok(ResponseHelper.Success<string>(result, LogConst.UserRegisterSuccess));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseHelper.Failure(ex.Message));
        }
    }

    [HttpPost("GoogleLogin")]
    public async Task<ActionResult> AuthGoogleLogin(AuthLoginModel authLoginModel)
    {
        try
        {
            var result = await _AuthMicroService.GoogleAuthentication(authLoginModel);
            if (result == string.Empty)
                return NoContent();
            return Ok(ResponseHelper.Success<string>(result, LogConst.GoogleLoginSuccess));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseHelper.Failure(ex.Message));
        }
    }

    [HttpPost("UserLogin")]
    public async Task<ActionResult> UserLogin(LoginCheck loginCheck)
    {
        try
        {
            var result = await _AuthMicroService.UserLogin(loginCheck);
            return Ok(ResponseHelper.Success<string>(result, LogConst.UserLoginSuccess));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseHelper.Failure(ex.Message));
        }
    }

    [Authorize]
    [HttpGet("GetUserByEmail/{email}")]
    public async Task<ActionResult> GetUserByEmail(string email)
    {
        try
        {
            var user = await _AuthMicroService.GetUserByEmail(email);
            if (user == null)
                return NotFound();
            return Ok(ResponseHelper.Success(user, LogConst.GetUserByEmailSuccess));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseHelper.Failure(ex.Message));
        }
    }
}