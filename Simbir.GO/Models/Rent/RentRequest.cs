using Simbir.GO.Models.Transport;

namespace Simbir.GO.Controllers.Rent;

public class RentRequest
{
    public required double Lat;
    public required double Long;
    public required double Radius;
    public required TransportType TransportType;
}