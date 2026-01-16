namespace BiografOpgave.Domain.Models;

public class Screen
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string City { get; set; }
    public int Rows { get; set; }
    public int SeatsPerRow { get; set; }
    public ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
}
