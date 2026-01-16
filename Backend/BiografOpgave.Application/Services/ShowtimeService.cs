namespace BiografOpgave.Application.Services;

public class ShowtimeService : IShowtimeService
{
    private readonly IShowtimeRepository _showtimes;

    public ShowtimeService(IShowtimeRepository showtimes)
    {
        _showtimes = showtimes;
    }

    public async Task<IEnumerable<ShowtimeDTOResponse>> GetAll()
        => (await _showtimes.GetAll()).Select(ToDto);

    public async Task<ShowtimeDTOResponse?> GetById(int id)
        => (await _showtimes.GetById(id)) is { } showtime ? ToDto(showtime) : null;

    public async Task<ShowtimeDTOResponse?> Create(ShowtimeDTORequest showtime)
    {
        var created = await _showtimes.Create(ToEntity(showtime));
        return ToDto(created);
    }

    public async Task<ShowtimeDTOResponse?> Update(ShowtimeDTORequest showtime)
    {
        var existing = await _showtimes.GetById(showtime.Id);
        if (existing == null) return null;

        existing.MovieId = showtime.MovieId;
        existing.ScreenId = showtime.ScreenId;
        existing.StartTime = showtime.StartTime;
        existing.BasePrice = showtime.BasePrice;
        existing.Is3D = showtime.Is3D;
        existing.Language = showtime.Language;

        var updated = await _showtimes.Update(existing);
        return updated == null ? null : ToDto(updated);
    }

    public Task<bool> Delete(int id) => _showtimes.Delete(id);

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
