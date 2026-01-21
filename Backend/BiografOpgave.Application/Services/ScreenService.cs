namespace BiografOpgave.Application.Services;

// Application service der håndterer forretningslogik for biografsale (screens).
// Ligger i Application-laget og fungerer som mellemled mellem controller og repository.
public class ScreenService : IScreenService
{
    // Repository-interface til dataadgang for screens
    private readonly IScreenRepository _screens;

    // Dependency Injection:
    // IScreenRepository injiceres via constructoren
    public ScreenService(IScreenRepository screens)
    {
        _screens = screens;
    }

    // Henter alle screens og mapper dem til response-DTO'er
    public async Task<IEnumerable<ScreenDTOResponse>> GetAll()
        => (await _screens.GetAll()).Select(ToDto);

    // Henter én screen baseret på id
    // Returnerer null hvis screen ikke findes
    public async Task<ScreenDTOResponse?> GetById(int id)
        => (await _screens.GetById(id)) is { } screen ? ToDto(screen) : null;

    // Opretter en ny screen
    public async Task<ScreenDTOResponse?> Create(ScreenDTORequest screen)
    {
        // Simpel validering: navn må ikke være tomt
        if (string.IsNullOrWhiteSpace(screen.Name)) return null;

        // Mapper request-DTO til entity og gemmer den via repository
        var created = await _screens.Create(ToEntity(screen));

        // Mapper entity til response-DTO
        return ToDto(created);
    }

    // Opdaterer en eksisterende screen
    public async Task<ScreenDTOResponse?> Update(ScreenDTORequest screen)
    {
        // Finder eksisterende entity
        var existing = await _screens.GetById(screen.Id);
        if (existing == null) return null;

        // Opdaterer felter ud fra request-DTO
        existing.Name = screen.Name;
        existing.City = screen.City;
        existing.Rows = screen.Rows;
        existing.SeatsPerRow = screen.SeatsPerRow;

        // Gemmer ændringerne
        var updated = await _screens.Update(existing);

        // Returnerer opdateret DTO eller null hvis opdateringen fejlede
        return updated == null ? null : ToDto(updated);
    }

    // Sletter en screen baseret på id
    public Task<bool> Delete(int id) => _screens.Delete(id);

    // Mapper Screen entity til ScreenDTOResponse
    // Bruges når data sendes fra backend til frontend
    private static ScreenDTOResponse ToDto(Screen screen) => new()
    {
        Id = screen.Id,
        Name = screen.Name,
        City = screen.City,
        Rows = screen.Rows,
        SeatsPerRow = screen.SeatsPerRow
    };

    // Mapper ScreenDTORequest til Screen entity
    // Bruges når data modtages fra frontend og gemmes i databasen
    private static Screen ToEntity(ScreenDTORequest screen) => new()
    {
        Id = screen.Id,
        Name = screen.Name,
        City = screen.City,
        Rows = screen.Rows,
        SeatsPerRow = screen.SeatsPerRow
    };
}
