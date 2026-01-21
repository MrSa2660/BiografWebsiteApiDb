namespace BiografOpgave.Application.DTOs;

// DTO til input (Create og Update).
// Definerer hvilke felter klienten må sende til API'et.
// Undgå at tage imod en database-entity direkt
public class MovieDTORequest
{
    // Bruges ved Update for at identificere hvilken film der skal opdateres.
    public int Id { get; set; }
    // Titel er påkrævet ved oprettelse og opdatering.
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
    // Showtimes modtaget som tekst fra klienten (fx CSV eller visningsformat).
    public string? Showtimes { get; set; }
    public bool IsHighlight { get; set; }
}
