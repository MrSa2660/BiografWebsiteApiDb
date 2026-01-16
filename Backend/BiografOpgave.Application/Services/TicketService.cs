namespace BiografOpgave.Application.Services;

public class TicketService : ITicketService
{
    private readonly ITicketRepository _tickets;

    public TicketService(ITicketRepository tickets)
    {
        _tickets = tickets;
    }

    public async Task<IEnumerable<TicketDTOResponse>> GetForBooking(int bookingId)
        => (await _tickets.GetForBooking(bookingId)).Select(ToDto);

    public async Task<IEnumerable<TicketDTOResponse>> GetForUser(int userId)
        => (await _tickets.GetForUser(userId)).Select(ToDto);

    public async Task<TicketDTOResponse?> GetById(int id)
        => (await _tickets.GetById(id)) is { } ticket ? ToDto(ticket) : null;

    public async Task<TicketDTOResponse?> Create(TicketDTORequest ticket)
    {
        var entity = new Ticket
        {
            BookingId = ticket.BookingId,
            Code = ticket.Code,
            SeatLabel = ticket.SeatLabel,
            IssuedAt = ticket.IssuedAt
        };

        var created = await _tickets.Create(entity);
        return ToDto(created);
    }

    public Task<bool> Delete(int id) => _tickets.Delete(id);

    private static TicketDTOResponse ToDto(Ticket ticket) => new()
    {
        Id = ticket.Id,
        BookingId = ticket.BookingId,
        Code = ticket.Code,
        SeatLabel = ticket.SeatLabel,
        IssuedAt = ticket.IssuedAt
    };
}
