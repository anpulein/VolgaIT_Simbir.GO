namespace Simbir.GO.Services;

public interface IBCryptNet
{
    /// <summary>
    /// Get hash password
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    string GetPasswordHash(string password);
    
    /// <summary>
    /// Checking password hash
    /// </summary>
    /// <param name="reqPassword"></param>
    /// <param name="dbPassport"></param>
    /// <returns></returns>
    bool IsPasswordHash(string reqPassword, string dbPassport);
}