namespace BiografOpgave.Domain.Models;

public class Booking
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ShowtimeId { get; set; }
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
    public Showtime? Showtime { get; set; }
    public ICollection<BookingSeat> Seats { get; set; } = new List<BookingSeat>();
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
