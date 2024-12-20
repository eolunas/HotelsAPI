using Microsoft.OpenApi.Models;

public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Hotel Management API",
                Version = "v1",
                Description = "API for managing hotels and reservations."
            });
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Hotel Management API v1");
            options.RoutePrefix = string.Empty; // Hace que Swagger esté disponible en la raíz (http://localhost:<puerto>/)
        });

        return app;
    }
}
