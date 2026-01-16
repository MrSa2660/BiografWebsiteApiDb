namespace BiografOpgave.Application.DTOs;

public class ScreenDTOResponse
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string City { get; set; }
    public int Rows { get; set; }
    public int SeatsPerRow { get; set; }
}
