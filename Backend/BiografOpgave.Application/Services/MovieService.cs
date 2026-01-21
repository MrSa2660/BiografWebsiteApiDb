namespace BiografOpgave.Application.Services;

// Application service som indeholder forretningslogik for Movie.
// Ligger i Application-laget og fungerer som bindeled mellem controller og repository.
// Mapper mellem DTO'er (API-kontrakt) og Entities (datamodel).
public class MovieService : IMovieService
{
    // Repository-interface til dataadgang.
    // Bruger interface for at opnå løs kobling og bedre testbarhed.
    private readonly IMovieRepository _movies;

    // Constructor injection:
    // Repository bliver injiceret af ASP.NET Core's DI-container.
    public MovieService(IMovieRepository movies)
    {
        _movies = movies;
    }

    // Henter alle film fra databasen.
    // Resultatet fra repository (entities) mappes til DTO'er før de returneres til controlleren.
    public async Task<IEnumerable<MovieDTOResponse>> GetAll()
        => (await _movies.GetAll()).Select(ToDto);

    // Henter en film baseret på id.
    // Hvis filmen findes, mappes den til en DTO.
    // Hvis den ikke findes, returneres null (controller oversætter til 404).
    public async Task<MovieDTOResponse?> GetById(int id)
        => (await _movies.GetById(id)) is { } movie ? ToDto(movie) : null;

    // Opretter en ny film.
    // Indeholder simpel forretningsvalidering (Title må ikke være tom).
    public async Task<MovieDTOResponse?> Create(MovieDTORequest movie)
    {
        // Simpel runtime-validering af input
        if (string.IsNullOrWhiteSpace(movie.Title)) return null;

        // Mapper request-DTO til entity og gemmer den via repository
        var created = await _movies.Create(ToEntity(movie));

        // Mapper den gemte entity tilbage til response-DTO
        return ToDto(created);
    }

    // Opdaterer en eksisterende film.
    public async Task<MovieDTOResponse?> Update(MovieDTORequest movie)
    {
        // Finder eksisterende entity i databasen
        var existing = await _movies.GetById(movie.Id);
        if (existing == null) return null;

        // Opdaterer entity-felter med data fra request-DTO
        existing.Title = movie.Title;
        existing.Description = movie.Description;
        existing.Genre = movie.Genre;
        existing.DurationMinutes = movie.DurationMinutes;
        existing.Language = movie.Language;
        existing.Rating = movie.Rating;
        existing.PosterUrl = movie.PosterUrl;
        existing.TrailerUrl = movie.TrailerUrl;
        existing.ReleaseDate = movie.ReleaseDate;
        existing.IsNowShowing = movie.IsNowShowing;
        existing.ShowtimesCsv = movie.Showtimes;
        existing.IsHighlight = movie.IsHighlight;

        // Gemmer ændringerne via repository
        var updated = await _movies.Update(existing);

        // Returnerer opdateret DTO eller null hvis opdateringen fejlede
        return updated == null ? null : ToDto(updated);
    }

    // Sletter en film baseret på id.
    // Returnerer true/false så controlleren kan afgøre statuskode.
    public Task<bool> Delete(int id) => _movies.Delete(id);

    // Mapper Movie entity til MovieDTOResponse.
    // Bruges når data sendes fra backend til frontend.
    private static MovieDTOResponse ToDto(Movie movie) => new()
    {
        Id = movie.Id,
        Title = movie.Title,
        Description = movie.Description,
        Genre = movie.Genre,
        DurationMinutes = movie.DurationMinutes,
        Language = movie.Language,
        Rating = movie.Rating,
        PosterUrl = movie.PosterUrl,
        TrailerUrl = movie.TrailerUrl,
        ReleaseDate = movie.ReleaseDate,
        IsNowShowing = movie.IsNowShowing,
        Showtimes = movie.ShowtimesCsv,
        IsHighlight = movie.IsHighlight
    };

    // Mapper MovieDTORequest til Movie entity.
    // Bruges når data modtages fra frontend og skal gemmes i databasen.
    private static Movie ToEntity(MovieDTORequest movie) => new()
    {
        Id = movie.Id,
        Title = movie.Title,
        Description = movie.Description,
        Genre = movie.Genre,
        DurationMinutes = movie.DurationMinutes,
        Language = movie.Language,
        Rating = movie.Rating,
        PosterUrl = movie.PosterUrl,
        TrailerUrl = movie.TrailerUrl,
        ReleaseDate = movie.ReleaseDate,
        IsNowShowing = movie.IsNowShowing,
        ShowtimesCsv = movie.Showtimes,
        IsHighlight = movie.IsHighlight
    };
}
