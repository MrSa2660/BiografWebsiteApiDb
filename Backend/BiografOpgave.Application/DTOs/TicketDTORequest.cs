namespace BiografOpgave.Application.DTOs;

public class TicketDTORequest
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public required string Code { get; set; }
    public required string SeatLabel { get; set; }
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
}
