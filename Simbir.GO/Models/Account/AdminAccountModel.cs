using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Simbir.GO.Models;

public class AdminAccountModel : AccountModel
{
    public bool IsAdmin { get; set; }
    
    [DefaultValue(0)]
    [Range(0, double.MaxValue, ErrorMessage = "Значение должно быть больше или равно 0")]
    public double Balance { get; set; }
}