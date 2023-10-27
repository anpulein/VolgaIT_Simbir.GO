using System.IdentityModel.Tokens.Jwt;
using Simbir.GO.Extensions;
using Simbir.GO.Models;

namespace Simbir.GO.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string CreateToken(Account account)
    {
        var token = account
            .CreateClaims()
            .CreateJwtToken(_configuration);
        var tokenHandler = new JwtSecurityTokenHandler();
        
        return tokenHandler.WriteToken(token);
    }
}