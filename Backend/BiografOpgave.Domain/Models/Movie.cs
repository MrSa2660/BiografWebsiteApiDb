namespace BiografOpgave.Domain.Models;

// Domain model (Entity) som repræsenterer en film i databasen.
// Bruges af EF Core til mapping til tabeller/kolonner.

public class Movie
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
    // Simpel lagring af showtimes som tekst/CSV.
    // Bruges til hurtig visning eller hvis man ikke vil normalisere showtimes i separate rows.
    public string? ShowtimesCsv { get; set; }
    public bool IsHighlight { get; set; }
    // Navigation property til relationen mellem Movie og Showtime (1-to-many).
    // Initialiseres for at undgå null og gøre det nemt at tilføje showtimes.
    public ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
}
