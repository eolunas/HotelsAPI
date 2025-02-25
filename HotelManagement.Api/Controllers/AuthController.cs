using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Authentication API - Endpoints for managing Auth and users")]
public class AuthController : ControllerBase
{
    private readonly IJwtService _jwtService;
    private readonly IAuthService _authService;

    public AuthController( IJwtService jwtService, IAuthService authService)
    {
        _jwtService = jwtService;
        _authService = authService;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <remarks>
    /// This endpoint allows a new user to create an account.
    /// </remarks>
    /// <param name="registrationDto">User registration data</param>
    /// <returns>JWT token for the registered user</returns>
    [HttpPost("register")]
    [SwaggerOperation(
        Summary = "Register a new user",
        Description = "Creates a new user account and returns a JWT token.",
        OperationId = "RegisterUser"
    )]
    [SwaggerResponse(200, "User registered successfully", typeof(AuthResultDto))]
    [SwaggerResponse(400, "Invalid registration request")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationDto registrationDto)
    {
        try
        {
            var newUser = await _authService.RegisterUserAsync(registrationDto);

            // Generate JWT token
            var token = _jwtService.GenerateToken(newUser.Id.ToString(), newUser.Email, newUser.Role.ToString());

            return Ok(new AuthResultDto { Token = token, Expiration = DateTime.UtcNow.AddMinutes(60) });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Authenticate a user and return a JWT token
    /// </summary>
    /// <remarks>
    /// This endpoint allows registered users to log in and receive a JWT token.
    /// </remarks>
    /// <param name="loginDto">User login credentials</param>
    /// <returns>JWT token for the authenticated user</returns>
    [HttpPost("login")]
    [SwaggerOperation(
        Summary = "User login",
        Description = "Authenticates a user and returns a JWT token.",
        OperationId = "LoginUser"
    )]
    [SwaggerResponse(200, "User authenticated successfully", typeof(AuthResultDto))]
    [SwaggerResponse(401, "Unauthorized - Invalid credentials")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
    {
        try
        {
            var userLogged = await _authService.LoginUserAsync(loginDto);

            // Generate JWT token
            var token = _jwtService.GenerateToken(userLogged.Id.ToString(), userLogged.Email, userLogged.Role.ToString());

            return Ok(new AuthResultDto { Token = token, Expiration = DateTime.UtcNow.AddMinutes(60) });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }
}
