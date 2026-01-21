namespace BiografOpgave.Application.Services;

// Application service der håndterer booking-logik.
// Indeholder forretningsregler for oprettelse af bookinger, sæder og billetter.
// Kommunikerer med databasen via repository-interfaces.
public class BookingService : IBookingService
{
    // Repository til håndtering af bookings
    private readonly IBookingRepository _bookings;

    // Repository til håndtering af tickets
    private readonly ITicketRepository _tickets;

    // Dependency Injection:
    // Repositories injiceres via constructoren
    public BookingService(IBookingRepository bookings, ITicketRepository tickets)
    {
        _bookings = bookings;
        _tickets = tickets;
    }

    // Henter alle bookinger og mapper dem til response-DTO'er
    public async Task<IEnumerable<BookingDTOResponse>> GetAll()
        => (await _bookings.GetAll()).Select(ToDto);

    // Henter én booking med detaljer (sæder + billetter)
    // Returnerer null hvis bookingen ikke findes
    public async Task<BookingDTOResponse?> GetById(int id)
        => (await _bookings.GetDetailed(id)) is { } booking ? ToDto(booking) : null;

    // Henter alle bookinger for en bestemt bruger
    public async Task<IEnumerable<BookingDTOResponse>> GetForUser(int userId)
        => (await _bookings.GetForUser(userId)).Select(ToDto);

    // Henter allerede reserverede sæder for et showtime
    // Bruges af frontend til at deaktivere optagede pladser
    public async Task<IEnumerable<BookingSeatDTO>> GetSeatsForShowtime(int showtimeId)
        => (await _bookings.GetSeatsForShowtime(showtimeId))
            .Select(s => new BookingSeatDTO
            {
                Row = s.Row,
                Number = s.Number,
                Price = s.Price
            });

    // Opretter en ny booking
    public async Task<BookingDTOResponse?> Create(BookingDTORequest request)
    {
        // Grundlæggende validering:
        // Brugeren skal være gyldig, og der skal vælges mindst ét sæde
        if (request.UserId <= 0 || !request.Seats.Any()) return null;

        // Henter allerede bookede sæder for showtime
        var existingSeats = await _bookings.GetSeatsForShowtime(request.ShowtimeId);

        // Opretter et hash-set af ønskede sæder (Row-Number)
        // Bruges til hurtigt overlap-tjek
        var requestedSeats = request.Seats
            .Select(s => $"{s.Row}-{s.Number}")
            .ToHashSet(StringComparer.Ordinal);

        // Hvis et af de ønskede sæder allerede er booket → afvis booking
        if (existingSeats.Any(s => requestedSeats.Contains($"{s.Row}-{s.Number}")))
            return null;

        // Opretter Booking-entity med sæder
        var entity = new Booking
        {
            UserId = request.UserId,
            ShowtimeId = request.ShowtimeId,
            Status = BookingStatus.Confirmed,
            Seats = request.Seats.Select(s => new BookingSeat
            {
                Row = s.Row,
                Number = s.Number,
                Price = s.Price
            }).ToList()
        };

        // Beregner totalpris baseret på valgte sæder
        entity.TotalPrice = entity.Seats.Sum(s => s.Price);

        // Gemmer bookingen i databasen
        var created = await _bookings.Create(entity);

        // Opretter billetter for hver reserveret plads
        await CreateTickets(created);

        // Henter booking igen med alle relationer (tickets m.m.)
        var detailed = await _bookings.GetDetailed(created.Id) ?? created;

        // Mapper til response-DTO og returnerer
        return ToDto(detailed);
    }

    // Opdaterer status på en booking (fx Pending → Confirmed)
    public async Task<BookingDTOResponse?> UpdateStatus(int id, BookingStatus status)
    {
        // Finder eksisterende booking
        var existing = await _bookings.GetDetailed(id);
        if (existing == null) return null;

        // Opdaterer status
        existing.Status = status;

        // Gemmer ændringen
        var updated = await _bookings.Update(existing);
        if (updated == null) return null;

        // Hvis en booking bliver bekræftet senere,
        // og der endnu ikke er udstedt billetter,
        // oprettes billetter nu
        if (status == BookingStatus.Confirmed && !updated.Tickets.Any() && updated.Seats.Any())
        {
            await CreateTickets(updated);
            updated = await _bookings.GetDetailed(updated.Id) ?? updated;
        }

        return ToDto(updated);
    }

    // Sletter en booking
    public Task<bool> Delete(int id) => _bookings.Delete(id);

    // Opretter billetter for hver plads i bookingen
    private async Task CreateTickets(Booking booking)
    {
        foreach (var seat in booking.Seats)
        {
            var ticket = new Ticket
            {
                BookingId = booking.Id,
                Code = GenerateTicketCode(),
                SeatLabel = $"Row {seat.Row} Seat {seat.Number}",
                IssuedAt = DateTime.UtcNow
            };

            // Gemmer billetten i databasen
            await _tickets.Create(ticket);
        }
    }

    // Genererer en kort, unik billetkode
    // Fx: A3F9C2B1
    private static string GenerateTicketCode()
        => Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();

    // Mapper Booking entity til BookingDTOResponse
    // Bruges når data sendes fra backend til frontend
    private static BookingDTOResponse ToDto(Booking booking) => new()
    {
        Id = booking.Id,
        UserId = booking.UserId,
        ShowtimeId = booking.ShowtimeId,
        TotalPrice = booking.TotalPrice,
        Status = booking.Status,
        CreatedAt = booking.CreatedAt,

        // Mapper sæder
        Seats = booking.Seats.Select(s => new BookingSeatDTO
        {
            Row = s.Row,
            Number = s.Number,
            Price = s.Price
        }).ToList(),

        // Mapper billetter
        Tickets = booking.Tickets.Select(t => new TicketDTOResponse
        {
            Id = t.Id,
            BookingId = t.BookingId,
            Code = t.Code,
            SeatLabel = t.SeatLabel,
            IssuedAt = t.IssuedAt
        }).ToList()
    };
}
