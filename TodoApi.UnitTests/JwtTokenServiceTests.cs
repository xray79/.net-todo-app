using System.IdentityModel.Tokens.Jwt;
using FluentAssertions;
using Microsoft.Extensions.Options;
using TodoApi.Auth;
using TodoApi.Entities;
using Xunit;

namespace TodoApi.UnitTests;

public class JwtTokenServiceTests
{
    [Fact]
    public void CreateToken_ShouldIncludeUserClaims()
    {
        var settings = Options.Create(new JwtSettings
        {
            Issuer = "TodoApi",
            Audience = "TodoUi",
            SigningKey = "test-signing-key-123456789012345678901",
            ExpiresMinutes = 60
        });

        var service = new JwtTokenService(settings);
        var user = new ApplicationUser { Id = "user-1", Email = "test@example.com" };

        var token = service.CreateToken(user);
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == "user-1");
        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == "test@example.com");
    }
}
