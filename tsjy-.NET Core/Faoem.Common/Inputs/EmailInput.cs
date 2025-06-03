using System.ComponentModel.DataAnnotations;

namespace Faoem.Common.Inputs;

public class EmailInput
{
    [RegularExpression("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$")]
    public string Email { get; set; } = null!;
}