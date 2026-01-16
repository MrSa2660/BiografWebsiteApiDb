namespace BiografOpgave.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;

    public UsersController(IUserService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDTOResponse>>> GetAll()
        => Ok(await _service.GetAll());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDTOResponse>> Get(int id)
    {
        var user = await _service.GetById(id);
        if (user == null) return NotFound();
        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDTOResponse>> Login(UserLoginRequest request)
    {
        var user = await _service.Authenticate(request.Email, request.Password);
        if (user == null) return Unauthorized();
        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<UserDTOResponse>> Create(UserDTORequest dto)
    {
        var created = await _service.Create(dto);
        if (created == null) return BadRequest();
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserDTOResponse>> Update(int id, UserDTORequest dto)
    {
        if (id != dto.Id) return BadRequest();
        var updated = await _service.Update(dto);
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
