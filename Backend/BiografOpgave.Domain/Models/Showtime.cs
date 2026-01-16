namespace BiografOpgave.Domain.Models;

public class Showtime
{
    public int Id { get; set; }
    public int MovieId { get; set; }
    public int ScreenId { get; set; }
    public DateTime StartTime { get; set; }
    public decimal BasePrice { get; set; }
    public bool Is3D { get; set; }
    public string? Language { get; set; }

    public Movie? Movie { get; set; }
    public Screen? Screen { get; set; }
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
