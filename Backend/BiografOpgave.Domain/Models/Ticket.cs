namespace BiografOpgave.Domain.Models;

public class Ticket
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public required string Code { get; set; }
    public required string SeatLabel { get; set; }
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

    public Booking? Booking { get; set; }
}
