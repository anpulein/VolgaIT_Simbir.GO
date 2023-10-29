using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Simbir.GO.Models;

public class Account
{
    /// <summary>
    /// User Id
    /// </summary>
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// User login
    /// </summary>
    [Required]
    public string Username { get; set; } = String.Empty;
    
    /// <summary>
    /// User password (Hash)
    /// </summary>
    [Required]
    public string Password { get; set; } = String.Empty;
    
    /// <summary>
    /// Is the users
    /// </summary>
    [DefaultValue(false)]
    public bool IsAdmin { get; set; }
    
    /// <summary>
    /// User balance
    /// </summary>
    [DefaultValue(0)]
    [Range(0, double.MaxValue, ErrorMessage = "Значение должно быть больше или равно 0")]
    public double Balance { get; set; }

    /// <summary>
    /// Refresh token
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Refresh token expiry time
    /// </summary>
    public DateTime? RefreshTokenExpiryTime { get; set; }
}