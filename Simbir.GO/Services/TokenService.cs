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

    public string CreateToken(AccountInfo accountInfo)
    {
        var token = accountInfo
            .CreateClaims()
            .CreateJwtToken(_configuration);
        var tokenHandler = new JwtSecurityTokenHandler();
        
        return tokenHandler.WriteToken(token);
    }
}