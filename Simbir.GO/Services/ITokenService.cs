using Simbir.GO.Models;

namespace Simbir.GO.Services;

public interface ITokenService
{
    /// <summary>
    /// Create token
    /// </summary>
    /// <param name="accountInfo"></param>
    /// <returns></returns>
    string CreateToken(AccountInfo accountInfo);
}