using AuthSystem.Application.DTOs;
using AuthSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuthSystem.Application.Services;
using AuthSystem.Core.Enums; // For Role enum
using Microsoft.AspNetCore.Cors;


namespace AuthSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableCors("AllowReactApp")]
[Authorize]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var response = await _authService.LoginAsync(loginDto);
        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var response = await _authService.RegisterAsync(registerDto);
        return Ok(response);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
    {
        var response = await _authService.RefreshTokenAsync(refreshTokenDto);
        return Ok(response);
    }

    [Authorize(Roles = Role.Admin)]
    [HttpGet("admin-only")]
    public IActionResult AdminOnly()
    {
        return Ok("This is accessible only by Admin");
    }

    [Authorize]
    [HttpGet("authenticated-only")]
    public IActionResult AuthenticatedOnly()
    {
        return Ok("This is accessible by any authenticated user");
    }
}