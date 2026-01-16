namespace BiografOpgave.Application.DTOs;

public class UserLoginResponse
{
    public required UserDTOResponse User { get; set; }
    public required string Token { get; set; }
}
