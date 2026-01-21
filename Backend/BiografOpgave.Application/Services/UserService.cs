namespace BiografOpgave.Application.Services;

// Application service der håndterer brugere og authentication-logik.
// Ligger i Application-laget og fungerer som mellemled mellem controller og repository.
// Indeholder både CRUD og simpel login-logik.
public class UserService : IUserService
{
    // Repository-interface til dataadgang for brugere
    private readonly IUserRepository _users;

    // Dependency Injection:
    // IUserRepository injiceres via constructoren
    public UserService(IUserRepository users)
    {
        _users = users;
    }

    // Henter alle brugere og mapper dem til response-DTO'er
    public async Task<IEnumerable<UserDTOResponse>> GetAll()
        => (await _users.GetAll()).Select(ToDto);

    // Henter én bruger baseret på id
    // Returnerer null hvis brugeren ikke findes
    public async Task<UserDTOResponse?> GetById(int id)
        => (await _users.GetById(id)) is { } user ? ToDto(user) : null;

    // Henter én bruger baseret på email
    // Bruges bl.a. ved login og validering
    public async Task<UserDTOResponse?> GetByEmail(string email)
        => (await _users.GetByEmail(email)) is { } user ? ToDto(user) : null;

    // Autentificerer en bruger (login)
    public async Task<UserDTOResponse?> Authenticate(string email, string password)
    {
        // Grundlæggende input-validering
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return null;

        // Finder brugeren baseret på email
        var user = await _users.GetByEmail(email);
        if (user == null) return null;

        // Hasher det indtastede password
        var passwordHash = HashPassword(password);

        // Sammenligner hash med det gemte hash i databasen
        return string.Equals(user.PasswordHash, passwordHash, StringComparison.Ordinal)
            ? ToDto(user)
            : null;
    }

    // Opretter en ny bruger
    public async Task<UserDTOResponse?> Create(UserDTORequest request)
    {
        // Simpel validering
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.FullName))
            return null;

        // Mapper request-DTO til User entity
        var entity = new User
        {
            Email = request.Email,
            FullName = request.FullName,
            Role = request.Role,

            // Password hashes før det gemmes i databasen
            PasswordHash = HashPassword(request.Password ?? string.Empty)
        };

        // Gemmer brugeren i databasen
        var created = await _users.Create(entity);

        // Mapper entity til response-DTO
        return ToDto(created);
    }

    // Opdaterer en eksisterende bruger
    public async Task<UserDTOResponse?> Update(UserDTORequest request)
    {
        // Finder eksisterende bruger
        var existing = await _users.GetById(request.Id);
        if (existing == null) return null;

        // Opdaterer brugerens grunddata
        existing.Email = request.Email;
        existing.FullName = request.FullName;
        existing.Role = request.Role;

        // Opdaterer kun password hvis der er sendt et nyt
        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            existing.PasswordHash = HashPassword(request.Password);
        }

        // Gemmer ændringerne
        var updated = await _users.Update(existing);

        // Returnerer opdateret DTO eller null hvis opdateringen fejlede
        return updated == null ? null : ToDto(updated);
    }

    // Sletter en bruger baseret på id
    public Task<bool> Delete(int id) => _users.Delete(id);

    // Mapper User entity til UserDTOResponse
    // Bruges når data sendes fra backend til frontend
    private static UserDTOResponse ToDto(User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        FullName = user.FullName,
        Role = user.Role,
        CreatedAt = user.CreatedAt
    };

    // Hasher et password med SHA-256
    // Bruges både ved oprettelse og login til sammenligning
    private static string HashPassword(string password)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
        var hash = System.Security.Cryptography.SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }
}