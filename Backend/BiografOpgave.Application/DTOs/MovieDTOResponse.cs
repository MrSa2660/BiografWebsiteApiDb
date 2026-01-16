namespace BiografOpgave.Application.DTOs;

public class MovieDTOResponse
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? Genre { get; set; }
    public int DurationMinutes { get; set; }
    public string? Language { get; set; }
    public string? Rating { get; set; }
    public string? PosterUrl { get; set; }
    public string? TrailerUrl { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public bool IsNowShowing { get; set; }
    public string? Cities { get; set; }
    public string? Showtimes { get; set; }
    public bool IsHighlight { get; set; }
}
