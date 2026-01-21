namespace BiografOpgave.Application.DTOs;

// DTO til output (API response).
// Bruges til at sende kontrollerede data fra backend til frontend.
public class MovieDTOResponse
{
    // Unik identifikator for filmen.
    public int Id { get; set; }
    // Titel returneres altid og er påkrævet.
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
    // Showtimes formateret til visning i frontend.
    public string? Showtimes { get; set; }
    public bool IsHighlight { get; set; }
}
