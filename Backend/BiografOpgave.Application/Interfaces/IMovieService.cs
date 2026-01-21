namespace BiografOpgave.Application.Interfaces;


//Den definerer hvilke operationer der kan udføres på film (CRUD), uden at afsløre hvordan det er implementeret.
//Controlleren afhænger af interfacet i stedet for en konkret klasse, hvilket giver løs kobling og gør løsningen mere testbar og nemmere at udvide.
public interface IMovieService
{
    // Henter alle film
    Task<IEnumerable<MovieDTOResponse>> GetAll();
    // Henter én film ud fra id
    Task<MovieDTOResponse?> GetById(int id);
    // Opretter en ny film ud fra input-DTO
    Task<MovieDTOResponse?> Create(MovieDTORequest movie);
    // Opdaterer en eksisterende film
    Task<MovieDTOResponse?> Update(MovieDTORequest movie);
    // Opdaterer en eksisterende film
    Task<bool> Delete(int id);
}
