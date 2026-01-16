namespace BiografOpgave.Domain.Interfaces;

public interface IMovieRepository
{
    Task<IEnumerable<Movie>> GetAll();
    Task<Movie?> GetById(int id);
    Task<Movie> Create(Movie movie);
    Task<Movie?> Update(Movie movie);
    Task<bool> Delete(int id);
}
