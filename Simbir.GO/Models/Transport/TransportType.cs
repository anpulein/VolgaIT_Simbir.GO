using System.ComponentModel;
using Microsoft.OpenApi.Attributes;

namespace Simbir.GO.Models.Transport;

public enum TransportType
{
    [Description("Машина")]
    Car, 
    [Description("Мотоцикл")]
    Bike, 
    [Description("Скутер")]
    Scooter, 
    [Description("Все")]
    All
}