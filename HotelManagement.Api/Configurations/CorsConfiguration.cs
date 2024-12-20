public static class CorsConfiguration
{
    public static IServiceCollection AddCorsPolicies(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

        return services;
    }

    public static IApplicationBuilder UseCorsPolicies(this IApplicationBuilder app)
    {
        app.UseCors("AllowAll");
        return app;
    }
}
