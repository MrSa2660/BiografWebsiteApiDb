namespace BiografOpgave.Application.Interfaces;

public interface IMovieService
{
    Task<IEnumerable<MovieDTOResponse>> GetAll();
    Task<MovieDTOResponse?> GetById(int id);
    Task<MovieDTOResponse?> Create(MovieDTORequest movie);
    Task<MovieDTOResponse?> Update(MovieDTORequest movie);
    Task<bool> Delete(int id);
}
