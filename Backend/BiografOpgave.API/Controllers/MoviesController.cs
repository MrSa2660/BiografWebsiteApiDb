namespace BiografOpgave.API.Controllers;

// Marker klassen som en API-controller.
// Giver automatisk model binding, validering og bedre fejlrespons.
[ApiController]

// Definerer routen: /api/movies
// [controller] bliver automatisk erstattet med "movies"
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    // Service-interface som controlleren bruger.
    // Controlleren kender ikke repository eller database direkte.
    private readonly IMovieService _service;

    // Dependency Injection:
    // IMovieService injiceres via constructoren af DI-containeren.
    public MoviesController(IMovieService service)
    {
        _service = service;
    }

    // GET /api/movies
    // Henter alle film
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovieDTOResponse>>> GetAll()
        // Returnerer 200 OK med listen af film
        => Ok(await _service.GetAll());

    // GET /api/movies/{id}
    // Henter én film baseret på id
    [HttpGet("{id:int}")]
    public async Task<ActionResult<MovieDTOResponse>> Get(int id)
    {
        // Kalder service-laget for at hente filmen
        var movie = await _service.GetById(id);

        // Hvis filmen ikke findes, returneres 404 NotFound
        if (movie == null) return NotFound();

        // Hvis filmen findes, returneres 200 OK med data
        return Ok(movie);
    }

    // POST /api/movies
    // Opretter en ny film
    [HttpPost]

    // Custom authorization attribute:
    // Kun brugere med admin-rettigheder må oprette film
    [AdminOnly]
    public async Task<ActionResult<MovieDTOResponse>> Create(MovieDTORequest dto)
    {
        // Kalder service-laget for at oprette filmen
        var created = await _service.Create(dto);

        // Hvis input er ugyldigt, returneres 400 BadRequest
        if (created == null) return BadRequest();

        // Returnerer 201 Created + Location header
        // Peger på GET-endpointet for den nye film
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    // PUT /api/movies/{id}
    // Opdaterer en eksisterende film
    [HttpPut("{id:int}")]
    [AdminOnly]
    public async Task<ActionResult<MovieDTOResponse>> Update(int id, MovieDTORequest dto)
    {
        // Sikkerhedstjek: route-id skal matche dto.Id
        if (id != dto.Id) return BadRequest();

        // Kalder service-laget for at opdatere filmen
        var updated = await _service.Update(dto);

        // Hvis filmen ikke findes, returneres 404 NotFound
        if (updated == null) return NotFound();

        // Hvis opdatering lykkes, returneres 200 OK
        return Ok(updated);
    }

    // DELETE /api/movies/{id}
    // Sletter en film
    [HttpDelete("{id:int}")]
    [AdminOnly]
    public async Task<IActionResult> Delete(int id)
    {
        // Forsøger at slette filmen via service-laget
        var deleted = await _service.Delete(id);

        // Hvis filmen ikke findes, returneres 404 NotFound
        if (!deleted) return NotFound();

        // Hvis sletning lykkes, returneres 204 NoContent
        return NoContent();
    }
}