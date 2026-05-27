using Microsoft.AspNetCore.Identity;

namespace NuevoForo.Infrastructure.Services;

public static class RoleSeeder
{
    public static async Task SeedAsync(RoleManager<IdentityRole<Guid>> roleManager, CancellationToken cancellationToken = default)
    {
        var roles = new[] { "Usuario", "Moderador", "Admin" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"No se pudo crear el rol '{role}': {errors}");
                }
            }
        }
    }
}
