namespace BiografOpgave.Application.Interfaces;

public interface IShowtimeService
{
    Task<IEnumerable<ShowtimeDTOResponse>> GetAll();
    Task<ShowtimeDTOResponse?> GetById(int id);
    Task<ShowtimeDTOResponse?> Create(ShowtimeDTORequest showtime);
    Task<ShowtimeDTOResponse?> Update(ShowtimeDTORequest showtime);
    Task<bool> Delete(int id);
}
