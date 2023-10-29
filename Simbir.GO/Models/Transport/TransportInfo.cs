using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Simbir.GO.Models.Transport;

public class TransportInfo
{
    /// <summary>
    /// TransportInfo Id
    /// </summary>
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// Ref at account own
    /// </summary>
    [Required]
    public long OwnerId { get; set; } 

    /// <summary>
    /// Account 
    /// </summary>
    [ForeignKey("OwnerId")]
    public Account Owner { get; set; } 

    /// <summary>
    /// Is rented?
    /// </summary>
    [Required]
    public bool CanBeRented { get; set; }

    /// <summary>
    /// Type transport
    /// </summary>
    [Required]
    [EnumDataType(typeof(TransportType))]
    public TransportType TransportType { get; set; }

    /// <summary>
    /// Model
    /// </summary>
    [Required]
    public string Model { get; set; }

    /// <summary>
    /// Color
    /// </summary>
    [Required]
    public string Color { get; set; }

    /// <summary>
    /// Number sign
    /// </summary>
    [Required]
    public string Identifier { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Geographical latitude of the transport location
    /// </summary>
    [Required]
    public double Latitude { get; set; }

    /// <summary>
    /// Geographical longitude of the transport location
    /// </summary>
    [Required]
    public double Longitude { get; set; }

    /// <summary>
    /// Rental price per minute
    /// </summary>
    public double? MinutePrice { get; set; }
    
    /// <summary>
    /// Rental price per day
    /// </summary>
    public double? DayPrice { get; set; }
}