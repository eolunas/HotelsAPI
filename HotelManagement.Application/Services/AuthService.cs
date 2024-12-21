public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> RegisterUserAsync(UserRegistrationDto registrationDto)
    {

        // Check if the user already exists
        var existingUser = (await _userRepository.GetAllAsync())
            .FirstOrDefault(u => u.Email == registrationDto.Email);

        if (existingUser != null)
            throw new InvalidOperationException("User already exists.");

        // Hash the password
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registrationDto.Password);

        var newUser = new User
        {
            Email = registrationDto.Email,
            PasswordHash = hashedPassword,
            Role = registrationDto.Role
        };

        await _userRepository.AddAsync(newUser);

        return newUser;
    }

    public async Task<User> LoginUserAsync(UserLoginDto loginDto)
    {

        // Find the user
        var user = (await _userRepository.GetAllAsync())
            .FirstOrDefault(u => u.Email == loginDto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return user;
    }
}
