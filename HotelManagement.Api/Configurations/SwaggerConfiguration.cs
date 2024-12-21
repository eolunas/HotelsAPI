using Microsoft.OpenApi.Models;

public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.EnableAnnotations();

            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Hotel Management API",
                Version = "v1",
                Description = "A comprehensive API for managing hotel operations and reservations. This API supports CRUD operations for hotels, rooms, and reservations, enabling functionality such as room availability tracking, user role-based access control, and streamlined reservation processes. Ideal for modern hotel management systems, it ensures secure, efficient, and scalable operations for administrators, guests, and other stakeholders."
            });

            // Add JWT Authentication to Swagger
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer' [space] and then your token. Example: Bearer abc123"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>() // No roles/policies required for basic JWT validation
                }
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
            options.RoutePrefix = string.Empty; // Swagger at root path
        });

        return app;
    }
}
