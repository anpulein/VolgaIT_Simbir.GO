using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Simbir.GO.Models.Transport;

namespace Simbir.GO.Models.Rent;

public class Rental
{
    /// <summary>
    /// Rent id
    /// </summary>
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// Transport id
    /// </summary>
    [Required]
    public long TransportId { get; set; }

    /// <summary>
    /// Transport model
    /// </summary>
    [ForeignKey("TransportId")]
    public TransportInfo Transport { get; set; }

    /// <summary>
    /// Account id
    /// </summary>
    [Required]
    public long AccountId { get; set; }
    
    /// <summary>
    /// Account model
    /// </summary>
    [ForeignKey("AccountId")]
    public AccountInfo Account { get; set; }

    /// <summary>
    /// Start rent
    /// </summary>
    public DateTime TimeStart { get; set; } 

    /// <summary>
    /// End rent
    /// </summary>
    public DateTime? TimeEnd { get; set; }

    /// <summary>
    /// Price
    /// </summary>
    public double PriceOfUnit { get; set; }

    /// <summary>
    /// Type price
    /// </summary>
    [EnumDataType(typeof(PriceType))]
    public PriceType PriceType { get; set; }

    /// <summary>
    /// Final price
    /// </summary>
    public double? FinalPrice { get; set; }
}