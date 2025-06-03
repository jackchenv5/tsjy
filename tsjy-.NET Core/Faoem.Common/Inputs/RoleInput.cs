using System.ComponentModel.DataAnnotations;

namespace Faoem.Common.Inputs;

public class RoleInput
{
    [RegularExpression("^[a-zA-Z][a-zA-Z0-9_]{3,31}$")]
    public string RoleName { get; set; } = null!;
}