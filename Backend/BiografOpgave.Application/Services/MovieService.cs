namespace BiografOpgave.Application.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movies;

    public MovieService(IMovieRepository movies)
    {
        _movies = movies;
    }

    public async Task<IEnumerable<MovieDTOResponse>> GetAll()
        => (await _movies.GetAll()).Select(ToDto);

    public async Task<MovieDTOResponse?> GetById(int id)
        => (await _movies.GetById(id)) is { } movie ? ToDto(movie) : null;

    public async Task<MovieDTOResponse?> Create(MovieDTORequest movie)
    {
        if (string.IsNullOrWhiteSpace(movie.Title)) return null;

        var created = await _movies.Create(ToEntity(movie));
        return ToDto(created);
    }

    public async Task<MovieDTOResponse?> Update(MovieDTORequest movie)
    {
        var existing = await _movies.GetById(movie.Id);
        if (existing == null) return null;

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
        existing.CitiesCsv = movie.Cities;
        existing.ShowtimesCsv = movie.Showtimes;
        existing.IsHighlight = movie.IsHighlight;

        var updated = await _movies.Update(existing);
        return updated == null ? null : ToDto(updated);
    }

    public Task<bool> Delete(int id) => _movies.Delete(id);

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
        Cities = movie.CitiesCsv,
        Showtimes = movie.ShowtimesCsv,
        IsHighlight = movie.IsHighlight
    };

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
        CitiesCsv = movie.Cities,
        ShowtimesCsv = movie.Showtimes,
        IsHighlight = movie.IsHighlight
    };
}
