using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetBasePath(AppContext.BaseDirectory);
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                      .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
                      .AddEnvironmentVariables();

// PORT congiguration:
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5024); 
});

// JWT Configuration:
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddApplicationServices(); // Dependencies
builder.Services.AddSwaggerDocumentation(); // Swagger config
builder.Services.AddCorsPolicies(); // CORS Policities

// DB Connection:
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// DB Migration [for docker]
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ApplicationDbContext>();
    try
    {
        // Verify migration and execute auto:
        if (dbContext.Database.GetPendingMigrations().Any())
        {
            Console.WriteLine("Applying migrations...");
            dbContext.Database.Migrate();
        }

        Console.WriteLine("Executing SeedData...");
        SeedData.InitializeAsync(services).Wait();
        Console.WriteLine("SeedData completed.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error in DB: {ex.Message}");
        throw;
    }
}

// This is for deploy, not include swagger:
//if (app.Environment.IsDevelopment())
app.UseSwaggerDocumentation(); 

app.UseCorsPolicies();    // CORS Policities
app.UseAuthentication();  // Middleware for Auth
app.UseAuthorization();

app.MapControllers();

app.Run();