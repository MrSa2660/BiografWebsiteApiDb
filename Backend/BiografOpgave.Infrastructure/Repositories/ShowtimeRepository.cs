namespace BiografOpgave.Infrastructure.Repositories;

public class ShowtimeRepository : IShowtimeRepository
{
    private readonly DatabaseContext _context;

    public ShowtimeRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Showtime>> GetAll()
        => await _context.Showtimes.AsNoTracking().ToListAsync();

    public Task<Showtime?> GetById(int id)
        => _context.Showtimes.FindAsync(id).AsTask();

    public async Task<Showtime> Create(Showtime showtime)
    {
        _context.Showtimes.Add(showtime);
        await _context.SaveChangesAsync();
        return showtime;
    }

    public async Task<Showtime?> Update(Showtime showtime)
    {
        _context.Showtimes.Update(showtime);
        await _context.SaveChangesAsync();
        return showtime;
    }

    public async Task<bool> Delete(int id)
    {
        var entity = await _context.Showtimes.FindAsync(id);
        if (entity == null) return false;
        _context.Showtimes.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
