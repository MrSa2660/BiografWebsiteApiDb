namespace BiografOpgave.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _users;

    public UserService(IUserRepository users)
    {
        _users = users;
    }

    public async Task<IEnumerable<UserDTOResponse>> GetAll()
        => (await _users.GetAll()).Select(ToDto);

    public async Task<UserDTOResponse?> GetById(int id)
        => (await _users.GetById(id)) is { } user ? ToDto(user) : null;

    public async Task<UserDTOResponse?> GetByEmail(string email)
        => (await _users.GetByEmail(email)) is { } user ? ToDto(user) : null;

    public async Task<UserDTOResponse?> Authenticate(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return null;

        var user = await _users.GetByEmail(email);
        if (user == null) return null;

        var passwordHash = HashPassword(password);
        return string.Equals(user.PasswordHash, passwordHash, StringComparison.Ordinal)
            ? ToDto(user)
            : null;
    }

    public async Task<UserDTOResponse?> Create(UserDTORequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.FullName))
            return null;

        var entity = new User
        {
            Email = request.Email,
            FullName = request.FullName,
            Role = request.Role,
            PasswordHash = HashPassword(request.Password ?? string.Empty)
        };

        var created = await _users.Create(entity);
        return ToDto(created);
    }

    public async Task<UserDTOResponse?> Update(UserDTORequest request)
    {
        var existing = await _users.GetById(request.Id);
        if (existing == null) return null;

        existing.Email = request.Email;
        existing.FullName = request.FullName;
        existing.Role = request.Role;
        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            existing.PasswordHash = HashPassword(request.Password);
        }

        var updated = await _users.Update(existing);
        return updated == null ? null : ToDto(updated);
    }

    public Task<bool> Delete(int id) => _users.Delete(id);

    private static UserDTOResponse ToDto(User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        FullName = user.FullName,
        Role = user.Role,
        CreatedAt = user.CreatedAt
    };

    private static string HashPassword(string password)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
        var hash = System.Security.Cryptography.SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }
}
