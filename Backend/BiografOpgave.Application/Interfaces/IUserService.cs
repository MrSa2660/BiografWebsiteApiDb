namespace BiografOpgave.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDTOResponse>> GetAll();
    Task<UserDTOResponse?> GetById(int id);
    Task<UserDTOResponse?> GetByEmail(string email);
    Task<UserDTOResponse?> Authenticate(string email, string password);
    Task<UserDTOResponse?> Create(UserDTORequest user);
    Task<UserDTOResponse?> Update(UserDTORequest user);
    Task<bool> Delete(int id);
}
