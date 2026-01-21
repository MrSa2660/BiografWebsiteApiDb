namespace BiografOpgave.Application.Services;

// Application service der håndterer billetter (tickets).
// Ligger i Application-laget og fungerer som mellemled mellem controller og repository.
// Indeholder ingen kompleks forretningslogik, men styrer flow og mapping.
public class TicketService : ITicketService
{
    // Repository-interface til dataadgang for tickets
    private readonly ITicketRepository _tickets;

    // Dependency Injection:
    // ITicketRepository injiceres via constructoren
    public TicketService(ITicketRepository tickets)
    {
        _tickets = tickets;
    }

    // Henter alle billetter for en bestemt booking
    // Bruges fx når man vil se alle billetter til én reservation
    public async Task<IEnumerable<TicketDTOResponse>> GetForBooking(int bookingId)
        => (await _tickets.GetForBooking(bookingId)).Select(ToDto);

    // Henter alle billetter for en bestemt bruger
    // Typisk brugt til "mine billetter"-visning i frontend
    public async Task<IEnumerable<TicketDTOResponse>> GetForUser(int userId)
        => (await _tickets.GetForUser(userId)).Select(ToDto);

    // Henter én billet baseret på id
    // Returnerer null hvis billetten ikke findes
    public async Task<TicketDTOResponse?> GetById(int id)
        => (await _tickets.GetById(id)) is { } ticket ? ToDto(ticket) : null;

    // Opretter en ny billet
    // Bruges primært internt (fx fra BookingService),
    // ikke nødvendigvis direkte fra frontend
    public async Task<TicketDTOResponse?> Create(TicketDTORequest ticket)
    {
        // Mapper request-DTO til Ticket entity
        var entity = new Ticket
        {
            BookingId = ticket.BookingId,
            Code = ticket.Code,
            SeatLabel = ticket.SeatLabel,
            IssuedAt = ticket.IssuedAt
        };

        // Gemmer billetten i databasen
        var created = await _tickets.Create(entity);

        // Mapper entity til response-DTO
        return ToDto(created);
    }

    // Sletter en billet baseret på id
    // Returnerer true/false så controlleren kan afgøre statuskode
    public Task<bool> Delete(int id) => _tickets.Delete(id);

    // Mapper Ticket entity til TicketDTOResponse
    // Bruges når data sendes fra backend til frontend
    private static TicketDTOResponse ToDto(Ticket ticket) => new()
    {
        Id = ticket.Id,
        BookingId = ticket.BookingId,
        Code = ticket.Code,
        SeatLabel = ticket.SeatLabel,
        IssuedAt = ticket.IssuedAt
    };
}
