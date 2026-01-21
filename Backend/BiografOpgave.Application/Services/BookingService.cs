namespace BiografOpgave.Application.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookings;
    private readonly ITicketRepository _tickets;

    public BookingService(IBookingRepository bookings, ITicketRepository tickets)
    {
        _bookings = bookings;
        _tickets = tickets;
    }

    public async Task<IEnumerable<BookingDTOResponse>> GetAll()
        => (await _bookings.GetAll()).Select(ToDto);

    public async Task<BookingDTOResponse?> GetById(int id)
        => (await _bookings.GetDetailed(id)) is { } booking ? ToDto(booking) : null;

    public async Task<IEnumerable<BookingDTOResponse>> GetForUser(int userId)
        => (await _bookings.GetForUser(userId)).Select(ToDto);

    public async Task<IEnumerable<BookingSeatDTO>> GetSeatsForShowtime(int showtimeId)
        => (await _bookings.GetSeatsForShowtime(showtimeId))
            .Select(s => new BookingSeatDTO
            {
                Row = s.Row,
                Number = s.Number,
                Price = s.Price
            });

    public async Task<BookingDTOResponse?> Create(BookingDTORequest request)
    {
        if (request.UserId <= 0 || !request.Seats.Any()) return null;

        var existingSeats = await _bookings.GetSeatsForShowtime(request.ShowtimeId);
        var requestedSeats = request.Seats
            .Select(s => $"{s.Row}-{s.Number}")
            .ToHashSet(StringComparer.Ordinal);

        if (existingSeats.Any(s => requestedSeats.Contains($"{s.Row}-{s.Number}")))
            return null;

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
        entity.TotalPrice = entity.Seats.Sum(s => s.Price);

        var created = await _bookings.Create(entity);
        await CreateTickets(created);

        var detailed = await _bookings.GetDetailed(created.Id) ?? created;
        return ToDto(detailed);
    }

    public async Task<BookingDTOResponse?> UpdateStatus(int id, BookingStatus status)
    {
        var existing = await _bookings.GetDetailed(id);
        if (existing == null) return null;
        existing.Status = status;
        var updated = await _bookings.Update(existing);
        if (updated == null) return null;

        // issue tickets if a pending booking was confirmed later
        if (status == BookingStatus.Confirmed && !updated.Tickets.Any() && updated.Seats.Any())
        {
            await CreateTickets(updated);
            updated = await _bookings.GetDetailed(updated.Id) ?? updated;
        }

        return ToDto(updated);
    }

    public Task<bool> Delete(int id) => _bookings.Delete(id);

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
            await _tickets.Create(ticket);
        }
    }

    private static string GenerateTicketCode()
        => Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();

    private static BookingDTOResponse ToDto(Booking booking) => new()
    {
        Id = booking.Id,
        UserId = booking.UserId,
        ShowtimeId = booking.ShowtimeId,
        TotalPrice = booking.TotalPrice,
        Status = booking.Status,
        CreatedAt = booking.CreatedAt,
        Seats = booking.Seats.Select(s => new BookingSeatDTO
        {
            Row = s.Row,
            Number = s.Number,
            Price = s.Price
        }).ToList(),
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