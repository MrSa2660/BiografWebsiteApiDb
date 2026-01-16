namespace BiografOpgave.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DatabaseContext _context;

    public UserRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAll()
        => await _context.Users.AsNoTracking().ToListAsync();

    public Task<User?> GetById(int id)
        => _context.Users.FindAsync(id).AsTask();

    public Task<User?> GetByEmail(string email)
        => _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User> Create(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> Update(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> Delete(int id)
    {
        var entity = await _context.Users.FindAsync(id);
        if (entity == null) return false;
        _context.Users.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
