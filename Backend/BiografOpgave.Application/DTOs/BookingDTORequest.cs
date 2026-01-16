namespace BiografOpgave.Application.DTOs;

public class BookingDTORequest
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ShowtimeId { get; set; }
    public IEnumerable<BookingSeatDTO> Seats { get; set; } = Enumerable.Empty<BookingSeatDTO>();
}
