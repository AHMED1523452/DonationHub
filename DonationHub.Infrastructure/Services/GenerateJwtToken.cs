
using DonationHub.Application.Interfaces;
using DonationHub.Infrastructure.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DonationHub.Infrastructure.Services
{
    public class GenerateJwtToken : IGenerateJwtToken 
    {
        private readonly IOptions<JWTSettings> options;

        public GenerateJwtToken(IOptions<JWTSettings> options)
        {
            this.options = options;
        }

        public async Task<string> GenerateAccessTokenAsync(string Id,
                                                           string Email,
                                                           string? UserName,
                                                           string FullName,
                                                           string? role, CancellationToken cancellationToken = default)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(options.Value.SecretKEY);

            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, Id),
            new(JwtRegisteredClaimNames.Email, Email),
            new(JwtRegisteredClaimNames.Email,UserName),
            new(JwtRegisteredClaimNames.Name, FullName),
            new(ClaimTypes.NameIdentifier, Id)
        };
            //. Validate for existing Role in the DB or not 
            if (role is not null)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            else
            {
                // Default role fallback
                claims.Add(new Claim(ClaimTypes.Role, "User"));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(options.Value.ExpiryMinutes),
                Issuer = options.Value.Issuer,
                Audience = options.Value.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<string> GenerateRefreshTokenAsync(string Id,
                                                            string Email,
                                                            string? UserName,
                                                            string FullName,
                                                            string? role, CancellationToken cancellationToken = default)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(options.Value.SecretKEY);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, Id),
                new(JwtRegisteredClaimNames.Email, Email),
                new(JwtRegisteredClaimNames.Email, UserName),
                new(JwtRegisteredClaimNames.Name, FullName),
                new(ClaimTypes.NameIdentifier, Id)
            };
            if (role is not null)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            else
            {
                // Default role fallback
                claims.Add(new Claim(ClaimTypes.Role, "User"));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(options.Value.RefreshTokenExpiryDays),
                Issuer = options.Value.Issuer,
                Audience = options.Value.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
