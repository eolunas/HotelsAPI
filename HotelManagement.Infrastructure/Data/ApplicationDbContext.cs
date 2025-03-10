﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public class ApplicationDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration) : base(options) 
    {
        _configuration = configuration;
    }

    public ApplicationDbContext() { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Guest> Guests { get; set; }
    public DbSet<EmergencyContact> EmergencyContacts { get; set; }
    public DbSet<Country> Countries { get; set; } 
    public DbSet<Location> Locations { get; set; } 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Convert Role to string:
        modelBuilder.Entity<User>()
        .Property(u => u.Role)
        .HasConversion<string>();

        // Relation Room → Hotel (Cascade delete HotelId)
        modelBuilder.Entity<Room>()
            .HasOne(r => r.Hotel)
            .WithMany(h => h.Rooms)
            .HasForeignKey(r => r.HotelId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relation Room → User (Cascade delete restricted)
        modelBuilder.Entity<Room>()
            .HasOne(r => r.CreatedByUser)
            .WithMany(u => u.CreatedRooms)
            .HasForeignKey(r => r.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict); 

        // DateOnly configuration support:
        modelBuilder.Entity<Reservation>(entity =>
        {
            // CheckInDate:
            entity.Property(e => e.CheckInDate)
                .HasConversion(
                    v => v.ToDateTime(TimeOnly.MinValue), 
                    v => DateOnly.FromDateTime(v)        
                )
                .HasColumnType("date"); 

            // CheckOutDate:
            entity.Property(e => e.CheckOutDate)
                .HasConversion(
                    v => v.ToDateTime(TimeOnly.MinValue),
                    v => DateOnly.FromDateTime(v)
                )
                .HasColumnType("date");
        });

        modelBuilder.Entity<Guest>(entity =>
        {
            // BirthDate:
            entity.Property(e => e.BirthDate)
                .HasConversion(
                    v => v.ToDateTime(TimeOnly.MinValue), 
                    v => DateOnly.FromDateTime(v)        
                )
                .HasColumnType("date"); 
        });

        modelBuilder.Entity<Location>()
            .HasOne(l => l.Country)
            .WithMany(c => c.Locations)
            .HasForeignKey(l => l.CountryId)
            .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelBuilder);

    }

}
