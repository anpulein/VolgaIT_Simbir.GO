
namespace Simbir.GO.Models;

public class AccountResponse
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public string Token { get; set; } = String.Empty;
    public string RefreshToken { get; set; } = String.Empty;
}