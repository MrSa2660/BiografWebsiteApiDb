namespace BiografOpgave.Domain.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAll();
    Task<User?> GetById(int id);
    Task<User?> GetByEmail(string email);
    Task<User> Create(User user);
    Task<User?> Update(User user);
    Task<bool> Delete(int id);
}
