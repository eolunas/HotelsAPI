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
        services.AddScoped<IEmailService, EmailService>();

        // Infraestructure repositories:
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRepository<Hotel>, HotelRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IGuestRepository, GuestRepository>();
        services.AddScoped<IEmergencyContactRepository, EmergencyContactRepository>();

        return services;
    }
}
