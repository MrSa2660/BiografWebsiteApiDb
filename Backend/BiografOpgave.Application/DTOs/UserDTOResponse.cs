namespace BiografOpgave.Application.DTOs;

public class UserDTOResponse
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string FullName { get; set; }
    public string Role { get; set; } = "User";
    public DateTime CreatedAt { get; set; }
}
