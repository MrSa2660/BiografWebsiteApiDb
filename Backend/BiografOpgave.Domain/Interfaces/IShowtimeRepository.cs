namespace BiografOpgave.Domain.Interfaces;

public interface IShowtimeRepository
{
    Task<IEnumerable<Showtime>> GetAll();
    Task<Showtime?> GetById(int id);
    Task<Showtime> Create(Showtime showtime);
    Task<Showtime?> Update(Showtime showtime);
    Task<bool> Delete(int id);
}
