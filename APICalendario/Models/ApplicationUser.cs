
using Microsoft.AspNetCore.Identity;

namespace APICalendario.Models;

public class ApplicationUser : IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpirityTime { get; set; }
}
