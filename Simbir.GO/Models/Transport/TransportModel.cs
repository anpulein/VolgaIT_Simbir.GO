namespace Simbir.GO.Models.Transport;

public class TransportModel
{
    public required bool CanBeRented { get; set; }
    public required TransportType TransportType { get; set; }
    public required string Model { get; set; }
    public required string Color { get; set; }
    public required string Identifier { get; set; }
    public string? Description { get; set; }
    public required double Latitude { get; set; }
    public required double Longitude { get; set; }
    public double? MinutePrice { get; set; }
    public double? DayPrice { get; set; }
}