using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace BiografOpgave.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _service;

    public BookingsController(IBookingService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookingDTOResponse>>> GetAll()
        => Ok(await _service.GetAll());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookingDTOResponse>> Get(int id)
    {
        var booking = await _service.GetById(id);
        if (booking == null) return NotFound();
        return Ok(booking);
    }

    [Authorize]
    [HttpGet("user/{userId:int}")]
    public async Task<ActionResult<IEnumerable<BookingDTOResponse>>> GetForUser(int userId)
    {
        var principal = HttpContext.User;
        var role = principal.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
        var sub = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var isAdmin = string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase);

        if (!isAdmin && (!int.TryParse(sub, out var requesterId) || requesterId != userId))
            return Forbid();

        return Ok(await _service.GetForUser(userId));
    }

    [HttpGet("showtime/{showtimeId:int}/seats")]
    public async Task<ActionResult<IEnumerable<BookingSeatDTO>>> GetSeatsForShowtime(int showtimeId)
        => Ok(await _service.GetSeatsForShowtime(showtimeId));

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<BookingDTOResponse>> Create(BookingDTORequest dto)
    {
        var principal = HttpContext.User;
        var sub = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(sub, out var requesterId))
            return Unauthorized();

        // enforce the booking user matches the token
        dto.UserId = requesterId;

        var created = await _service.Create(dto);
        if (created == null) return BadRequest();
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<BookingDTOResponse>> UpdateStatus(int id, [FromBody] BookingStatus status)
    {
        var updated = await _service.UpdateStatus(id, status);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.Delete(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
