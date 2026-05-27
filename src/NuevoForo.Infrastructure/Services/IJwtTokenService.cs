using NuevoForo.Domain.Entities;

namespace NuevoForo.Infrastructure.Services;

public interface IJwtTokenService
{
    string GenerateToken(Usuario usuario, IList<string> roles);
}
