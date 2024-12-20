using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IJwtService _jwtService;
    private readonly IAuthService _authService;

    public AuthController( IJwtService jwtService, IAuthService authService)
    {
        _jwtService = jwtService;
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationDto registrationDto)
    {
        try
        {
            var newUser = await _authService.RegisterUserAsync(registrationDto);

            // Generate JWT token
            var token = _jwtService.GenerateToken(newUser.Id.ToString(), newUser.Email, newUser.Role);

            return Ok(new AuthResultDto { Token = token, Expiration = DateTime.UtcNow.AddMinutes(60) });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
    {
        try
        {
            var userLogged = await _authService.LoginUserAsync(loginDto);

            // Generate JWT token
            var token = _jwtService.GenerateToken(userLogged.Id.ToString(), userLogged.Email, userLogged.Role);

            return Ok(new AuthResultDto { Token = token, Expiration = DateTime.UtcNow.AddMinutes(60) });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }
}
