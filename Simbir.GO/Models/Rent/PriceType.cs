using System.ComponentModel;

namespace Simbir.GO.Models.Rent;

public enum PriceType
{
    [Description("Минуты")]
    Minutes,
    [Description("Дни")]
    Days
}