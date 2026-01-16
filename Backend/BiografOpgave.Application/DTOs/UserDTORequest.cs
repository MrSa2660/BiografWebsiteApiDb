namespace BiografOpgave.Application.DTOs;

public class UserDTORequest
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string FullName { get; set; }
    public string Role { get; set; } = "User";
    public string? Password { get; set; }
}
