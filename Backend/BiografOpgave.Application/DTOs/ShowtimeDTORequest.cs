namespace BiografOpgave.Application.DTOs;

public class ShowtimeDTORequest
{
    public int Id { get; set; }
    public int MovieId { get; set; }
    public int ScreenId { get; set; }
    public DateTime StartTime { get; set; }
    public decimal BasePrice { get; set; }
    public bool Is3D { get; set; }
    public string? Language { get; set; }
}
