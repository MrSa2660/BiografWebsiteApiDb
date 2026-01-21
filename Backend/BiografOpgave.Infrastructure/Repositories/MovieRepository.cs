namespace BiografOpgave.Infrastructure.Repositories;

// Repository-klasse i Infrastructure-laget.
// Ansvarlig for al databaseadgang vedrørende Movie.
// Implementerer IMovieRepository og bruger EF Core via DatabaseContext.
public class MovieRepository : IMovieRepository
{
    // DbContext som giver adgang til databasen via EF Core.
    private readonly DatabaseContext _context;

    // Dependency Injection:
    // DatabaseContext injiceres via constructoren.
    public MovieRepository(DatabaseContext context)
    {
        _context = context;
    }

    // Henter alle film fra databasen.
    // AsNoTracking bruges, fordi data kun skal læses og ikke opdateres.
    // Det giver bedre performance.
    public async Task<IEnumerable<Movie>> GetAll()
        => await _context.Movies.AsNoTracking().ToListAsync();

    // Henter én film baseret på id.
    // FindAsync returnerer null, hvis filmen ikke findes.
    public Task<Movie?> GetById(int id)
        => _context.Movies.FindAsync(id).AsTask();

    // Opretter en ny film i databasen.
    public async Task<Movie> Create(Movie movie)
    {
        // Tilføjer entity til DbContext
        _context.Movies.Add(movie);

        // Gemmer ændringerne i databasen
        await _context.SaveChangesAsync();

        // Returnerer den oprettede entity (med genereret Id)
        return movie;
    }

    // Opdaterer en eksisterende film i databasen.
    public async Task<Movie?> Update(Movie movie)
    {
        // Markerer entity som opdateret
        _context.Movies.Update(movie);

        // Gemmer ændringerne i databasen
        await _context.SaveChangesAsync();

        // Returnerer den opdaterede entity
        return movie;
    }

    // Sletter en film baseret på id.
    public async Task<bool> Delete(int id)
    {
        // Finder filmen i databasen
        var entity = await _context.Movies.FindAsync(id);

        // Hvis filmen ikke findes, returneres false
        if (entity == null) return false;

        // Fjerner entity fra DbContext
        _context.Movies.Remove(entity);

        // Gemmer ændringerne i databasen
        await _context.SaveChangesAsync();

        // Returnerer true for at indikere at sletningen lykkedes
        return true;
    }
}
