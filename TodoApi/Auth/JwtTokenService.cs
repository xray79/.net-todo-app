using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TodoApi.Entities;

namespace TodoApi.Auth;

public class JwtSettings
{
    public string Issuer { get; set; } = "TodoApi";
    public string Audience { get; set; } = "TodoUi";
    public string SigningKey { get; set; } = "super-secret-signing-key-with-32chars";
    public int ExpiresMinutes { get; set; } = 120;
}

public interface IJwtTokenService
{
    string CreateToken(ApplicationUser user);
}

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _settings;

    public JwtTokenService(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
    }

    public string CreateToken(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email ?? string.Empty)
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SigningKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddMinutes(_settings.ExpiresMinutes);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
