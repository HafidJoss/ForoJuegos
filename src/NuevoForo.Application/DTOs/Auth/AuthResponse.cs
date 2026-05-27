namespace NuevoForo.Application.DTOs.Auth;

public sealed class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
}
