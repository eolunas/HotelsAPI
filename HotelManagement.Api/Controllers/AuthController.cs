using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IJwtService _jwtService;

    public AuthController(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] UserLoginDto loginDto)
    {
        // Replace with real authentication logic (database check, etc.)
        if (loginDto.Email == "admin@hotel.com" && loginDto.Password == "Admin123")
        {
            var token = _jwtService.GenerateToken("1", loginDto.Email, "Admin");
            return Ok(new { Token = token });
        }

        return Unauthorized("Invalid email or password.");
    }
}
