using Tsjy.Eunms;
using Tsjy.Models;

namespace Tsjy.Dtos;

public class CraftBindingDto
{
    public long Id { get; set; }
    public int? Index { get; set; }
    public long? FacilityId { get; set; }
    public List<TsjyCraftBinding> Children { get; set; } = [];
}