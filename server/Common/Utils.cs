using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;


namespace server.Common
{
    public class Utils
    {
        public static string Encrypt(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool Verify(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        public static string GenerateJWTToken(Guid userId, bool rememberMe)
        {
            IdentityOptions options = new IdentityOptions();
            // paramters of token regeneration
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]{
                        new Claim("userId", userId.ToString())
                    }
                ),
                Expires = rememberMe ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(
                   new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Secret Key")),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(securityToken);
        }
        public static string GenerateJWTTokenByEmail(string email)
        {
            IdentityOptions options = new IdentityOptions();
            // We will setup the parameters of token generation
            SecurityTokenDescriptor tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]{
                        new Claim("email", email)
                    }
                ),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Jwt Secret key")),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken = tokenHandler.CreateToken(tokenDescription);
            return tokenHandler.WriteToken(securityToken);
        }
    }
}