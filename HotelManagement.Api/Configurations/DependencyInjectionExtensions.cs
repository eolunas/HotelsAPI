public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Token generator:
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IAuthService, AuthService>();

        // Aplication services:
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IHotelService, HotelService>();
        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<ICountryService, CountryService>();
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<IEmailService, EmailService>();

        // Infraestructure repositories:
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IHotelRepository, HotelRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IGuestRepository, GuestRepository>();
        services.AddScoped<IEmergencyContactRepository, EmergencyContactRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<ICountryRepository, CountryRepository>();

        return services;
    }
}
