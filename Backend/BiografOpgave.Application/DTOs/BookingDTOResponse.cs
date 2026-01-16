namespace BiografOpgave.Application.DTOs;

public class BookingDTOResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ShowtimeId { get; set; }
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public IEnumerable<BookingSeatDTO> Seats { get; set; } = Enumerable.Empty<BookingSeatDTO>();
    public IEnumerable<TicketDTOResponse> Tickets { get; set; } = Enumerable.Empty<TicketDTOResponse>();
}
