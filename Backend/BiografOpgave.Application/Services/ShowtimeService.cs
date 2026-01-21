namespace BiografOpgave.Application.Services;

// Application service der håndterer forretningslogik for showtimes (visningstider).
// Ligger i Application-laget og fungerer som mellemled mellem controller og repository.
public class ShowtimeService : IShowtimeService
{
    // Repository-interface til dataadgang for showtimes
    private readonly IShowtimeRepository _showtimes;

    // Dependency Injection:
    // IShowtimeRepository injiceres via constructoren
    public ShowtimeService(IShowtimeRepository showtimes)
    {
        _showtimes = showtimes;
    }

    // Henter alle showtimes og mapper dem til response-DTO'er
    public async Task<IEnumerable<ShowtimeDTOResponse>> GetAll()
        => (await _showtimes.GetAll()).Select(ToDto);

    // Henter én showtime baseret på id
    // Returnerer null hvis showtime ikke findes
    public async Task<ShowtimeDTOResponse?> GetById(int id)
        => (await _showtimes.GetById(id)) is { } showtime ? ToDto(showtime) : null;

    // Opretter en ny showtime
    public async Task<ShowtimeDTOResponse?> Create(ShowtimeDTORequest showtime)
    {
        // Mapper request-DTO til entity og gemmer den via repository
        var created = await _showtimes.Create(ToEntity(showtime));

        // Mapper den oprettede entity til response-DTO
        return ToDto(created);
    }

    // Opdaterer en eksisterende showtime
    public async Task<ShowtimeDTOResponse?> Update(ShowtimeDTORequest showtime)
    {
        // Finder eksisterende entity
        var existing = await _showtimes.GetById(showtime.Id);
        if (existing == null) return null;

        // Opdaterer entity-felter ud fra request-DTO
        existing.MovieId = showtime.MovieId;
        existing.ScreenId = showtime.ScreenId;
        existing.StartTime = showtime.StartTime;
        existing.BasePrice = showtime.BasePrice;
        existing.Is3D = showtime.Is3D;
        existing.Language = showtime.Language;

        // Gemmer ændringerne via repository
        var updated = await _showtimes.Update(existing);

        // Returnerer opdateret DTO eller null hvis opdateringen fejlede
        return updated == null ? null : ToDto(updated);
    }

    // Sletter en showtime baseret på id
    public Task<bool> Delete(int id) => _showtimes.Delete(id);

    // Mapper Showtime entity til ShowtimeDTOResponse
    // Bruges når data sendes fra backend til frontend
    private static ShowtimeDTOResponse ToDto(Showtime showtime) => new()
    {
        Id = showtime.Id,
        MovieId = showtime.MovieId,
        ScreenId = showtime.ScreenId,
        StartTime = showtime.StartTime,
        BasePrice = showtime.BasePrice,
        Is3D = showtime.Is3D,
        Language = showtime.Language
    };

    // Mapper ShowtimeDTORequest til Showtime entity
    // Bruges når data modtages fra frontend og gemmes i databasen
    private static Showtime ToEntity(ShowtimeDTORequest showtime) => new()
    {
        Id = showtime.Id,
        MovieId = showtime.MovieId,
        ScreenId = showtime.ScreenId,
        StartTime = showtime.StartTime,
        BasePrice = showtime.BasePrice,
        Is3D = showtime.Is3D,
        Language = showtime.Language
    };
}
