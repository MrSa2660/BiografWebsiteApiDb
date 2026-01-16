namespace BiografOpgave.Domain.Models;

public class BookingSeat
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public int Row { get; set; }
    public int Number { get; set; }
    public decimal Price { get; set; }

    public Booking? Booking { get; set; }
}
