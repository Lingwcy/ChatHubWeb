using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChatHubApi.Services
{
    public static class jwtService
    {
        public static string CreateJwtToken(IEnumerable<Claim> claims, DateTime expiresAt,JwtSecurityTokenHandler handler,IConfiguration config)
        {
            var key = Encoding.ASCII.GetBytes(config.GetValue<string>("SecretKey") ?? "");
            var jwt = new JwtSecurityToken(
                 claims: claims,
                 notBefore: DateTime.UtcNow,
                 expires: expiresAt,
                 signingCredentials: new SigningCredentials(
                              new SymmetricSecurityKey(key),
                              SecurityAlgorithms.HmacSha256Signature
                              )
                );

            var token = handler.WriteToken(jwt);
            var Djwt = handler.ReadJwtToken(token);
            foreach (var claim in Djwt.Claims)
            {
                Console.WriteLine($"{claim.Value}");
            }
            return token;
        }
    }
}
