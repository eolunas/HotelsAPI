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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        // Relación Room → Hotel (Eliminar en cascada)
        modelBuilder.Entity<Room>()
            .HasOne(r => r.Hotel)
            .WithMany(h => h.Rooms)
            .HasForeignKey(r => r.HotelId)
            .OnDelete(DeleteBehavior.Cascade); // Mantén cascada aquí

        // Relación Room → User (Sin eliminación en cascada)
        modelBuilder.Entity<Room>()
            .HasOne(r => r.CreatedByUser)
            .WithMany(u => u.CreatedRooms)
            .HasForeignKey(r => r.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict); // Evita cascada aquí

        base.OnModelCreating(modelBuilder);

    }

}
