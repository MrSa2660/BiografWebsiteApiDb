namespace BiografOpgave.Domain.Interfaces;

public interface IScreenRepository
{
    Task<IEnumerable<Screen>> GetAll();
    Task<Screen?> GetById(int id);
    Task<Screen> Create(Screen screen);
    Task<Screen?> Update(Screen screen);
    Task<bool> Delete(int id);
}
