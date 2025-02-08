using DeveloperStore.Application.Abstractions;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Infra.Security.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DeveloperStore.Infra.Security.Authentication;

internal sealed class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
{
    public string Generate(User user)
    {
        var claims = new Claim[] {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };

        var key = Encoding.UTF8.GetBytes(options.Value.SecretKey);

        var symetricSecurityKey = new SymmetricSecurityKey(key);

        var signingCredentials = new SigningCredentials(symetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(options.Value.Issuer, options.Value.Audience, claims, null, DateTime.UtcNow.AddHours(1), signingCredentials);

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenValue;
    }
}