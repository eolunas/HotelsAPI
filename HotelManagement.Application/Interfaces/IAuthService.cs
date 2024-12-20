public interface IAuthService
{
    Task<User> RegisterUserAsync(UserRegistrationDto registrationDto);
    Task<User> LoginUserAsync(UserLoginDto loginDto);
}
