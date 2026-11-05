using Joker_app_backend.DTOs;
using Joker_app_backend.Models;
using Joker_app_backend.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Joker_app_backend.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthController:ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;

    public AuthController(UserManager<ApplicationUser> usermanager, ITokenService tokenservices)
    {
        _userManager = usermanager;
        _tokenService = tokenservices;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
    {
        var exsistingUser = await _userManager.FindByEmailAsync(dto.Email);

        if (exsistingUser != null)
        {
            return BadRequest(new { message = "email is already registered" });
        }

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        var token = _tokenService.CreateToken(user);

        return Ok(new AuthResponseDto
        {
            Token = token,
            Email = user.Email,
            FullName = user.FullName ?? string.Empty,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            return BadRequest(new { message = "email or password is worng" });
        }

        var isPassvalid = await _userManager.CheckPasswordAsync(user, dto.Password);

        if (!isPassvalid)
        {
            return Unauthorized(new { message = "email or password is worng" });
        }

        var token = _tokenService.CreateToken(user);

        return Ok(new AuthResponseDto
        {
            Token = token,
            Email = user.Email,
            FullName = user.FullName ?? string.Empty,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });
    }
}