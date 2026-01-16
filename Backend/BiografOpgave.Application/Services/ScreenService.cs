namespace BiografOpgave.Application.Services;

public class ScreenService : IScreenService
{
    private readonly IScreenRepository _screens;

    public ScreenService(IScreenRepository screens)
    {
        _screens = screens;
    }

    public async Task<IEnumerable<ScreenDTOResponse>> GetAll()
        => (await _screens.GetAll()).Select(ToDto);

    public async Task<ScreenDTOResponse?> GetById(int id)
        => (await _screens.GetById(id)) is { } screen ? ToDto(screen) : null;

    public async Task<ScreenDTOResponse?> Create(ScreenDTORequest screen)
    {
        if (string.IsNullOrWhiteSpace(screen.Name)) return null;

        var created = await _screens.Create(ToEntity(screen));
        return ToDto(created);
    }

    public async Task<ScreenDTOResponse?> Update(ScreenDTORequest screen)
    {
        var existing = await _screens.GetById(screen.Id);
        if (existing == null) return null;

        existing.Name = screen.Name;
        existing.City = screen.City;
        existing.Rows = screen.Rows;
        existing.SeatsPerRow = screen.SeatsPerRow;

        var updated = await _screens.Update(existing);
        return updated == null ? null : ToDto(updated);
    }

    public Task<bool> Delete(int id) => _screens.Delete(id);

    private static ScreenDTOResponse ToDto(Screen screen) => new()
    {
        Id = screen.Id,
        Name = screen.Name,
        City = screen.City,
        Rows = screen.Rows,
        SeatsPerRow = screen.SeatsPerRow
    };

    private static Screen ToEntity(ScreenDTORequest screen) => new()
    {
        Id = screen.Id,
        Name = screen.Name,
        City = screen.City,
        Rows = screen.Rows,
        SeatsPerRow = screen.SeatsPerRow
    };
}
