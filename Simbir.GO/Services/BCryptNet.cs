namespace Simbir.GO.Services;

public class BCryptNet : IBCryptNet
{
    public string GetPasswordHash(string password) => BCrypt.Net.BCrypt.HashPassword(password);

    public bool IsPasswordHash(string reqPassword, string dbPassport)
    {
        if (!BCrypt.Net.BCrypt.Verify(reqPassword, dbPassport))
        {
            return false;
        }

        return true;
    }
}