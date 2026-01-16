using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BiografOpgave.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;
    private readonly IConfiguration _config;

    public UsersController(IUserService service, IConfiguration config)
    {
        _service = service;
        _config = config;
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
    public async Task<ActionResult<UserLoginResponse>> Login(UserLoginRequest request)
    {
        var user = await _service.Authenticate(request.Email, request.Password);
        if (user == null) return Unauthorized();

        var token = GenerateJwt(user);
        return Ok(new UserLoginResponse
        {
            User = user,
            Token = token
        });
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

    private string GenerateJwt(UserDTOResponse user)
    {
        var key = _config["Jwt:Key"] ?? "dev-secret-key-change-me-32chars-min-2026!";
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role ?? "User")
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
