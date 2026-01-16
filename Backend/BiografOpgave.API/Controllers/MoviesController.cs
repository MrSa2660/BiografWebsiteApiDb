namespace BiografOpgave.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _service;

    public MoviesController(IMovieService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovieDTOResponse>>> GetAll()
        => Ok(await _service.GetAll());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MovieDTOResponse>> Get(int id)
    {
        var movie = await _service.GetById(id);
        if (movie == null) return NotFound();
        return Ok(movie);
    }

    [HttpPost]
    [AdminOnly]
    public async Task<ActionResult<MovieDTOResponse>> Create(MovieDTORequest dto)
    {
        var created = await _service.Create(dto);
        if (created == null) return BadRequest();
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [AdminOnly]
    public async Task<ActionResult<MovieDTOResponse>> Update(int id, MovieDTORequest dto)
    {
        if (id != dto.Id) return BadRequest();
        var updated = await _service.Update(dto);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    [AdminOnly]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.Delete(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
