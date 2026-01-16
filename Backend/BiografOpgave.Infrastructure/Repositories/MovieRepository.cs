namespace BiografOpgave.Infrastructure.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly DatabaseContext _context;

    public MovieRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Movie>> GetAll()
        => await _context.Movies.AsNoTracking().ToListAsync();

    public Task<Movie?> GetById(int id)
        => _context.Movies.FindAsync(id).AsTask();

    public async Task<Movie> Create(Movie movie)
    {
        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();
        return movie;
    }

    public async Task<Movie?> Update(Movie movie)
    {
        _context.Movies.Update(movie);
        await _context.SaveChangesAsync();
        return movie;
    }

    public async Task<bool> Delete(int id)
    {
        var entity = await _context.Movies.FindAsync(id);
        if (entity == null) return false;
        _context.Movies.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
