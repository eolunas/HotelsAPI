using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }


    public DbSet<User> Users { get; set; }
    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Guest> Guests { get; set; }
    public DbSet<EmergencyContact> EmergencyContacts { get; set; }
}
