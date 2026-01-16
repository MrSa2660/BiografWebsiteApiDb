namespace BiografOpgave.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScreensController : ControllerBase
{
    private readonly IScreenService _service;

    public ScreensController(IScreenService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ScreenDTOResponse>>> GetAll()
        => Ok(await _service.GetAll());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ScreenDTOResponse>> Get(int id)
    {
        var screen = await _service.GetById(id);
        if (screen == null) return NotFound();
        return Ok(screen);
    }

    [HttpPost]
    [AdminOnly]
    public async Task<ActionResult<ScreenDTOResponse>> Create(ScreenDTORequest dto)
    {
        var created = await _service.Create(dto);
        if (created == null) return BadRequest();
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [AdminOnly]
    public async Task<ActionResult<ScreenDTOResponse>> Update(int id, ScreenDTORequest dto)
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
