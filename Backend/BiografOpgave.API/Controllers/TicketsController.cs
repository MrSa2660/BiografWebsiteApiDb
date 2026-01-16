namespace BiografOpgave.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _service;

    public TicketsController(ITicketService service)
    {
        _service = service;
    }

    [HttpGet("booking/{bookingId:int}")]
    public async Task<ActionResult<IEnumerable<TicketDTOResponse>>> GetForBooking(int bookingId)
        => Ok(await _service.GetForBooking(bookingId));

    [HttpGet("user/{userId:int}")]
    public async Task<ActionResult<IEnumerable<TicketDTOResponse>>> GetForUser(int userId)
        => Ok(await _service.GetForUser(userId));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TicketDTOResponse>> Get(int id)
    {
        var ticket = await _service.GetById(id);
        if (ticket == null) return NotFound();
        return Ok(ticket);
    }

    [HttpPost]
    public async Task<ActionResult<TicketDTOResponse>> Create(TicketDTORequest dto)
    {
        var created = await _service.Create(dto);
        return CreatedAtAction(nameof(Get), new { id = created?.Id }, created);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.Delete(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
