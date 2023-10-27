using Simbir.GO.Models;

namespace Simbir.GO.Services;

public interface ITokenService
{
    /// <summary>
    /// Create token
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    string CreateToken(Account account);
}