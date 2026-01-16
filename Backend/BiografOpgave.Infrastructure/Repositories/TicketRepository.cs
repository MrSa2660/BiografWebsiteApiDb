namespace BiografOpgave.Infrastructure.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly DatabaseContext _context;

    public TicketRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Ticket>> GetForBooking(int bookingId)
        => await _context.Tickets
            .Where(t => t.BookingId == bookingId)
            .AsNoTracking()
            .ToListAsync();

    public async Task<IEnumerable<Ticket>> GetForUser(int userId)
        => await _context.Tickets
            .Include(t => t.Booking)
            .Where(t => t.Booking != null && t.Booking.UserId == userId)
            .AsNoTracking()
            .ToListAsync();

    public Task<Ticket?> GetById(int id)
        => _context.Tickets.FindAsync(id).AsTask();

    public async Task<Ticket> Create(Ticket ticket)
    {
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();
        return ticket;
    }

    public async Task<bool> Delete(int id)
    {
        var entity = await _context.Tickets.FindAsync(id);
        if (entity == null) return false;
        _context.Tickets.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
