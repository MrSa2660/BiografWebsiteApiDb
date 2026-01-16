namespace BiografOpgave.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShowtimesController : ControllerBase
{
    private readonly IShowtimeService _service;

    public ShowtimesController(IShowtimeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ShowtimeDTOResponse>>> GetAll()
        => Ok(await _service.GetAll());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ShowtimeDTOResponse>> Get(int id)
    {
        var showtime = await _service.GetById(id);
        if (showtime == null) return NotFound();
        return Ok(showtime);
    }

    [HttpPost]
    [AdminOnly]
    public async Task<ActionResult<ShowtimeDTOResponse>> Create(ShowtimeDTORequest dto)
    {
        var created = await _service.Create(dto);
        return CreatedAtAction(nameof(Get), new { id = created?.Id }, created);
    }

    [HttpPut("{id:int}")]
    [AdminOnly]
    public async Task<ActionResult<ShowtimeDTOResponse>> Update(int id, ShowtimeDTORequest dto)
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
