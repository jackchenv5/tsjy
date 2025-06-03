namespace Faoem.Common.Dtos;

public class UserDto
{
    public long Id { get; set; }
    public string Username { get; set; } = null!;
    public string LowerUsername { get; set; } = null!;
    public string? FullName { get; set; }
    public long CreatedAt { get; set; }
    public long LastLogin { get; set; }
    public string? Email { get; set; }
    public string? LowerEmail { get; set; }
}