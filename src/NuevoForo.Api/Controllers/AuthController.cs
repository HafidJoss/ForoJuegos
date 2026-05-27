using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuevoForo.Application.DTOs.Auth;
using NuevoForo.Domain.Entities;
using NuevoForo.Infrastructure.Services;

namespace NuevoForo.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager, IJwtTokenService tokenService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var existingByEmail = await userManager.FindByEmailAsync(request.Email);
        if (existingByEmail is not null)
        {
            return Conflict(new { message = "El email ya está registrado." });
        }

        var existingByUsername = await userManager.FindByNameAsync(request.Username);
        if (existingByUsername is not null)
        {
            return Conflict(new { message = "El nombre de usuario ya está registrado." });
        }

        var usuario = new Usuario
        {
            Email = request.Email,
            UserName = request.Username,
            Nombre = request.Nombre
        };

        var result = await userManager.CreateAsync(usuario, request.Password);
        if (!result.Succeeded)
        {
            return BadRequest(new { message = "No fue posible registrar el usuario.", errors = result.Errors });
        }

        await userManager.AddToRoleAsync(usuario, "Usuario");
        var roles = await userManager.GetRolesAsync(usuario);
        var token = tokenService.GenerateToken(usuario, roles);

        return Ok(new AuthResponse
        {
            Token = token,
            UserId = usuario.Id.ToString(),
            Email = usuario.Email ?? string.Empty,
            Username = usuario.UserName ?? string.Empty,
            Nombre = usuario.Nombre
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        Usuario? usuario;
        if (request.EmailOrUsername.Contains('@'))
        {
            usuario = await userManager.FindByEmailAsync(request.EmailOrUsername);
        }
        else
        {
            usuario = await userManager.FindByNameAsync(request.EmailOrUsername);
        }

        if (usuario is null)
        {
            return Unauthorized(new { message = "Credenciales inválidas." });
        }

        var signIn = await signInManager.CheckPasswordSignInAsync(usuario, request.Password, false);
        if (!signIn.Succeeded)
        {
            return Unauthorized(new { message = "Credenciales inválidas." });
        }

        var roles = await userManager.GetRolesAsync(usuario);
        var token = tokenService.GenerateToken(usuario, roles);

        return Ok(new AuthResponse
        {
            Token = token,
            UserId = usuario.Id.ToString(),
            Email = usuario.Email ?? string.Empty,
            Username = usuario.UserName ?? string.Empty,
            Nombre = usuario.Nombre
        });
    }

    [HttpPost("google")]
    public IActionResult GoogleLogin()
    {
        return StatusCode(StatusCodes.Status501NotImplemented, new { message = "Google OAuth pendiente de implementación." });
    }

    [HttpPost("forgot-password")]
    public IActionResult ForgotPassword()
    {
        return StatusCode(StatusCodes.Status501NotImplemented, new { message = "Recuperación de contraseña pendiente de implementación." });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<object>> Me()
    {
        var usuario = await userManager.GetUserAsync(User);
        if (usuario is null)
        {
            return Unauthorized();
        }

        var roles = await userManager.GetRolesAsync(usuario);

        return Ok(new
        {
            Id = usuario.Id,
            Email = usuario.Email ?? string.Empty,
            Username = usuario.UserName ?? string.Empty,
            Nombre = usuario.Nombre,
            Bio = usuario.Bio,
            AvatarUrl = usuario.AvatarUrl,
            FechaRegistro = usuario.FechaRegistro,
            Roles = roles
        });
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var usuario = await userManager.GetUserAsync(User);
        if (usuario is null)
        {
            return Unauthorized();
        }

        usuario.Nombre = request.Nombre.Trim();
        usuario.Bio = request.Bio?.Trim();

        var result = await userManager.UpdateAsync(usuario);
        if (!result.Succeeded)
        {
            return BadRequest(new { message = "No fue posible actualizar el perfil.", errors = result.Errors });
        }

        var roles = await userManager.GetRolesAsync(usuario);

        return Ok(new
        {
            Id = usuario.Id,
            Email = usuario.Email ?? string.Empty,
            Username = usuario.UserName ?? string.Empty,
            Nombre = usuario.Nombre,
            Bio = usuario.Bio,
            AvatarUrl = usuario.AvatarUrl,
            FechaRegistro = usuario.FechaRegistro,
            Roles = roles
        });
    }
}
