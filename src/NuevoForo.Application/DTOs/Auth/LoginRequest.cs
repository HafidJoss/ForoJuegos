namespace NuevoForo.Application.DTOs.Auth;

public sealed class LoginRequest
{
    public string EmailOrUsername { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
