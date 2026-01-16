namespace BiografOpgave.Infrastructure.Repositories;

public class ScreenRepository : IScreenRepository
{
    private readonly DatabaseContext _context;

    public ScreenRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Screen>> GetAll()
        => await _context.Screens.AsNoTracking().ToListAsync();

    public Task<Screen?> GetById(int id)
        => _context.Screens.FindAsync(id).AsTask();

    public async Task<Screen> Create(Screen screen)
    {
        _context.Screens.Add(screen);
        await _context.SaveChangesAsync();
        return screen;
    }

    public async Task<Screen?> Update(Screen screen)
    {
        _context.Screens.Update(screen);
        await _context.SaveChangesAsync();
        return screen;
    }

    public async Task<bool> Delete(int id)
    {
        var entity = await _context.Screens.FindAsync(id);
        if (entity == null) return false;
        _context.Screens.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
