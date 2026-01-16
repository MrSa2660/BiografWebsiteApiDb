namespace BiografOpgave.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly DatabaseContext _context;

    public BookingRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Booking>> GetAll()
        => await _context.Bookings
            .Include(b => b.Seats)
            .Include(b => b.Tickets)
            .AsNoTracking()
            .ToListAsync();

    public Task<Booking?> GetById(int id)
        => _context.Bookings.FindAsync(id).AsTask();

    public async Task<Booking?> GetDetailed(int id)
        => await _context.Bookings
            .Include(b => b.Seats)
            .Include(b => b.Tickets)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id);

    public async Task<IEnumerable<Booking>> GetForUser(int userId)
        => await _context.Bookings
            .Include(b => b.Seats)
            .Include(b => b.Tickets)
            .Where(b => b.UserId == userId)
            .AsNoTracking()
            .ToListAsync();

    public async Task<IEnumerable<BookingSeat>> GetSeatsForShowtime(int showtimeId)
        => await _context.BookingSeats
            .Include(bs => bs.Booking)
            .Where(bs =>
                bs.Booking != null &&
                bs.Booking.ShowtimeId == showtimeId &&
                bs.Booking.Status != BookingStatus.Cancelled &&
                bs.Booking.Status != BookingStatus.Failed)
            .AsNoTracking()
            .ToListAsync();

    public async Task<Booking> Create(Booking booking)
    {
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
        return booking;
    }

    public async Task<Booking?> Update(Booking booking)
    {
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
        return booking;
    }

    public async Task<bool> Delete(int id)
    {
        var entity = await _context.Bookings.FindAsync(id);
        if (entity == null) return false;
        _context.Bookings.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
