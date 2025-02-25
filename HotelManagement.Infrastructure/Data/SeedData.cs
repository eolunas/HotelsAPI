using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            await context.Database.MigrateAsync();

            // Create Admin User: 
            if (!context.Users.Any(u => u.Role == UserRole.Admin))
            {
                // Hash the password:
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword("admin123");

                context.Users.Add(new User
                {
                    Email = "admin@example.com",
                    PasswordHash = hashedPassword,
                    Role = UserRole.Admin
                });
                await context.SaveChangesAsync();
            }

            // Countries:
            if (!context.Countries.Any())
            {
                context.Countries.AddRange(
                    new Country { Name = "Colombia", CountryCode = "COL" },
                    new Country { Name = "Argentina", CountryCode = "ARG" },
                    new Country { Name = "Perú", CountryCode = "PER" },
                    new Country { Name = "Chile", CountryCode = "CHL" },
                    new Country { Name = "México", CountryCode = "MEX" }
                );
                await context.SaveChangesAsync();
            }

            // Locations:
            if (!context.Locations.Any())
            {
                context.Locations.AddRange(
                    // Colombia (CountryId = 1)
                    new Location { CityName = "Bogotá", CityCode = "BOG", CountryId = 1 },
                    new Location { CityName = "Medellín", CityCode = "MED", CountryId = 1 },
                    new Location { CityName = "Cali", CityCode = "CLO", CountryId = 1 },

                    // Argentina (CountryId = 2)
                    new Location { CityName = "Buenos Aires", CityCode = "BUE", CountryId = 2 },
                    new Location { CityName = "Córdoba", CityCode = "COR", CountryId = 2 },
                    new Location { CityName = "Rosario", CityCode = "ROS", CountryId = 2 },

                    // Perú (CountryId = 3)
                    new Location { CityName = "Lima", CityCode = "LIM", CountryId = 3 },
                    new Location { CityName = "Arequipa", CityCode = "AQP", CountryId = 3 },
                    new Location { CityName = "Cusco", CityCode = "CUS", CountryId = 3 }
                );
                await context.SaveChangesAsync();
            }

            // Hotels:
            if (!context.Hotels.Any())
            {
                context.Hotels.AddRange(
                    // Bogotá (LocationId = 1)
                    new Hotel { Name = "Hotel Bogotá Plaza", LocationId = 1, IsEnabled = true, CreatedByUserId = 1 },
                    new Hotel { Name = "JW Marriott Bogotá", LocationId = 1, IsEnabled = true, CreatedByUserId = 1 },

                    // Medellín (LocationId = 2)
                    new Hotel { Name = "Hotel Medellín Royal", LocationId = 2, IsEnabled = true, CreatedByUserId = 1 },
                    new Hotel { Name = "InterContinental Medellín", LocationId = 2, IsEnabled = true, CreatedByUserId = 1 },

                    // Cali (LocationId = 3)
                    new Hotel { Name = "Hotel Dann Cali", LocationId = 3, IsEnabled = true, CreatedByUserId = 1 },
                    new Hotel { Name = "Intercontinental Cali", LocationId = 3, IsEnabled = true, CreatedByUserId = 1 },

                    // Buenos Aires (LocationId = 4)
                    new Hotel { Name = "Alvear Palace Hotel", LocationId = 4, IsEnabled = true, CreatedByUserId = 1 },
                    new Hotel { Name = "Hotel Madero", LocationId = 4, IsEnabled = true, CreatedByUserId = 1 },

                    // Córdoba (LocationId = 5)
                    new Hotel { Name = "Sheraton Córdoba Hotel", LocationId = 5, IsEnabled = true, CreatedByUserId = 1 },
                    new Hotel { Name = "NH Córdoba Panorama", LocationId = 5, IsEnabled = true, CreatedByUserId = 1 },

                    // Rosario (LocationId = 6)
                    new Hotel { Name = "Esplendor Savoy Rosario", LocationId = 6, IsEnabled = true, CreatedByUserId = 1 },
                    new Hotel { Name = "Holiday Inn Rosario", LocationId = 6, IsEnabled = true, CreatedByUserId = 1 },

                    // Lima (LocationId = 7)
                    new Hotel { Name = "Belmond Miraflores Park", LocationId = 7, IsEnabled = true, CreatedByUserId = 1 },
                    new Hotel { Name = "JW Marriott Lima", LocationId = 7, IsEnabled = true, CreatedByUserId = 1 },

                    // Arequipa (LocationId = 8)
                    new Hotel { Name = "Casa Andina Premium Arequipa", LocationId = 8, IsEnabled = true, CreatedByUserId = 1 },
                    new Hotel { Name = "Costa del Sol Wyndham Arequipa", LocationId = 8, IsEnabled = true, CreatedByUserId = 1 },

                    // Cusco (LocationId = 9)
                    new Hotel { Name = "Palacio del Inka", LocationId = 9, IsEnabled = true, CreatedByUserId = 1 },
                    new Hotel { Name = "Belmond Hotel Monasterio", LocationId = 9, IsEnabled = false, CreatedByUserId = 1 }
                );
                await context.SaveChangesAsync();
            }

            // Rooms: 
            if (!context.Rooms.Any())
            {
                context.Rooms.AddRange(
                    // Hotel Bogotá Plaza (HotelId = 1)
                    new Room { RoomType = "Standard", BasePrice = 100, Taxes = 10, Location = "101", MaxNumberOfGuest = 2, IsAvailable = true, HotelId = 1, CreatedByUserId = 1 },
                    new Room { RoomType = "Deluxe", BasePrice = 150, Taxes = 15, Location = "102", MaxNumberOfGuest = 4, IsAvailable = true, HotelId = 1, CreatedByUserId = 1 },
                    new Room { RoomType = "Suite", BasePrice = 200, Taxes = 20, Location = "103", MaxNumberOfGuest = 6, IsAvailable = true, HotelId = 1, CreatedByUserId = 1 },

                    // JW Marriott Bogotá (HotelId = 2)
                    new Room { RoomType = "Standard", BasePrice = 110, Taxes = 11, Location = "201", MaxNumberOfGuest = 2, IsAvailable = true, HotelId = 2, CreatedByUserId = 1 },
                    new Room { RoomType = "Deluxe", BasePrice = 170, Taxes = 17, Location = "202", MaxNumberOfGuest = 4, IsAvailable = true, HotelId = 2, CreatedByUserId = 1 },
                    new Room { RoomType = "Suite", BasePrice = 220, Taxes = 22, Location = "203", MaxNumberOfGuest = 6, IsAvailable = true, HotelId = 2, CreatedByUserId = 1 },

                    // Hotel Medellín Royal (HotelId = 3)
                    new Room { RoomType = "Standard", BasePrice = 90, Taxes = 9, Location = "301", MaxNumberOfGuest = 2, IsAvailable = true, HotelId = 3, CreatedByUserId = 1 },
                    new Room { RoomType = "Deluxe", BasePrice = 140, Taxes = 14, Location = "302", MaxNumberOfGuest = 4, IsAvailable = true, HotelId = 3, CreatedByUserId = 1 },
                    new Room { RoomType = "Suite", BasePrice = 210, Taxes = 21, Location = "303", MaxNumberOfGuest = 6, IsAvailable = true, HotelId = 3, CreatedByUserId = 1 },

                    // InterContinental Medellín (HotelId = 4)
                    new Room { RoomType = "Standard", BasePrice = 95, Taxes = 9.5m, Location = "401", MaxNumberOfGuest = 2, IsAvailable = true, HotelId = 4, CreatedByUserId = 1 },
                    new Room { RoomType = "Deluxe", BasePrice = 160, Taxes = 16, Location = "402", MaxNumberOfGuest = 4, IsAvailable = true, HotelId = 4, CreatedByUserId = 1 },
                    new Room { RoomType = "Suite", BasePrice = 230, Taxes = 23, Location = "403", MaxNumberOfGuest = 6, IsAvailable = true, HotelId = 4, CreatedByUserId = 1 },

                    // Hotel Dann Cali (HotelId = 5)
                    new Room { RoomType = "Standard", BasePrice = 80, Taxes = 8, Location = "501", MaxNumberOfGuest = 2, IsAvailable = true, HotelId = 5, CreatedByUserId = 1 },
                    new Room { RoomType = "Deluxe", BasePrice = 130, Taxes = 13, Location = "502", MaxNumberOfGuest = 4, IsAvailable = true, HotelId = 5, CreatedByUserId = 1 },
                    new Room { RoomType = "Suite", BasePrice = 190, Taxes = 19, Location = "503", MaxNumberOfGuest = 6, IsAvailable = true, HotelId = 5, CreatedByUserId = 1 },

                    // Intercontinental Cali (HotelId = 6)
                    new Room { RoomType = "Standard", BasePrice = 100, Taxes = 10, Location = "601", MaxNumberOfGuest = 2, IsAvailable = true, HotelId = 6, CreatedByUserId = 1 },
                    new Room { RoomType = "Deluxe", BasePrice = 150, Taxes = 15, Location = "602", MaxNumberOfGuest = 4, IsAvailable = true, HotelId = 6, CreatedByUserId = 1 },
                    new Room { RoomType = "Suite", BasePrice = 210, Taxes = 21, Location = "603", MaxNumberOfGuest = 6, IsAvailable = true, HotelId = 6, CreatedByUserId = 1 },

                    // Alvear Palace Hotel (HotelId = 7)
                    new Room { RoomType = "Standard", BasePrice = 120, Taxes = 12, Location = "701", MaxNumberOfGuest = 2, IsAvailable = true, HotelId = 7, CreatedByUserId = 1 },
                    new Room { RoomType = "Deluxe", BasePrice = 180, Taxes = 18, Location = "702", MaxNumberOfGuest = 4, IsAvailable = true, HotelId = 7, CreatedByUserId = 1 },
                    new Room { RoomType = "Suite", BasePrice = 260, Taxes = 26, Location = "703", MaxNumberOfGuest = 6, IsAvailable = true, HotelId = 7, CreatedByUserId = 1 },

                    // Hotel Madero (HotelId = 8)
                    new Room { RoomType = "Standard", BasePrice = 110, Taxes = 11, Location = "801", MaxNumberOfGuest = 2, IsAvailable = true, HotelId = 8, CreatedByUserId = 1 },
                    new Room { RoomType = "Deluxe", BasePrice = 170, Taxes = 17, Location = "802", MaxNumberOfGuest = 4, IsAvailable = true, HotelId = 8, CreatedByUserId = 1 },
                    new Room { RoomType = "Suite", BasePrice = 240, Taxes = 24, Location = "803", MaxNumberOfGuest = 6, IsAvailable = false, HotelId = 8, CreatedByUserId = 1 },

                    // Sheraton Córdoba Hotel (HotelId = 9)
                    new Room { RoomType = "Standard", BasePrice = 90, Taxes = 9, Location = "901", MaxNumberOfGuest = 2, IsAvailable = true, HotelId = 9, CreatedByUserId = 1 },
                    new Room { RoomType = "Deluxe", BasePrice = 140, Taxes = 14, Location = "902", MaxNumberOfGuest = 4, IsAvailable = true, HotelId = 9, CreatedByUserId = 1 },
                    new Room { RoomType = "Suite", BasePrice = 200, Taxes = 20, Location = "903", MaxNumberOfGuest = 6, IsAvailable = false, HotelId = 9, CreatedByUserId = 1 }
                );

                await context.SaveChangesAsync();
            }

            // Guests:
            if (!context.Guests.Any())
            {
                var guests = new List<Guest>
                {
                    new() {
                        FullName = "Juan Pérez",
                        BirthDate = new DateOnly(1990, 5, 15),
                        Gender = "Male",
                        DocumentType = "Passport",
                        DocumentNumber = "A12345678",
                        Email = "juan.perez@example.com",
                        Phone = "3001234567"
                    },
                    new()
                    {
                        FullName = "María Gómez",
                        BirthDate = new DateOnly(1985, 3, 22),
                        Gender = "Female",
                        DocumentType = "NationalID",
                        DocumentNumber = "B98765432",
                        Email = "maria.gomez@example.com",
                        Phone = "3107654321"
                    },
                    new()
                    {
                        FullName = "Carlos López",
                        BirthDate = new DateOnly(1995, 7, 18),
                        Gender = "Male",
                        DocumentType = "Passport",
                        DocumentNumber = "C45678912",
                        Email = "carlos.lopez@example.com",
                        Phone = "3209998887"
                    },
                    new()
                    {
                        FullName = "Ana Torres",
                        BirthDate = new DateOnly(1998, 9, 30),
                        Gender = "Female",
                        DocumentType = "NationalID",
                        DocumentNumber = "D76543211",
                        Email = "ana.torres@example.com",
                        Phone = "3154446677"
                    },
                    new()
                    {
                        FullName = "Luis Méndez",
                        BirthDate = new DateOnly(1987, 2, 12),
                        Gender = "Male",
                        DocumentType = "NationalID",
                        DocumentNumber = "E87654321",
                        Email = "luis.mendez@example.com",
                        Phone = "3227778899"
                    }
                };

                context.Guests.AddRange(guests);
                context.SaveChanges();
            }

            if (!context.Reservations.Any())
            {
                var rooms = context.Rooms.Take(3).ToList();
                var hotels = context.Hotels.ToList();
                var guests = context.Guests.ToList();

                if (rooms.Count >= 3 && hotels.Any() && guests.Count >= 5)
                {
                    var reservations = new List<Reservation>
                    {
                        new()
                        {
                            CheckInDate = new DateOnly(2025, 4, 10),
                            CheckOutDate = new DateOnly(2025, 4, 15),
                            NumberOfGuests = 2,
                            IsConfirmed = true,
                            HotelId = hotels[0].Id,
                            RoomId = rooms[0].Id,
                            ReservationGuests = new List<ReservationGuest>
                            {
                                new() { GuestId = guests[0].Id },
                                new() { GuestId = guests[1].Id }
                            }
                        },
                        new()
                        {
                            CheckInDate = new DateOnly(2025, 5, 1),
                            CheckOutDate = new DateOnly(2025, 5, 5),
                            NumberOfGuests = 1,
                            IsConfirmed = true,
                            HotelId = hotels[1].Id,
                            RoomId = rooms[1].Id,
                            ReservationGuests = new List<ReservationGuest>
                            {
                                new() { GuestId = guests[2].Id }
                            }
                        },
                        new()
                        {
                            CheckInDate = new DateOnly(2025, 6, 15),
                            CheckOutDate = new DateOnly(2025, 6, 20),
                            NumberOfGuests = 3,
                            IsConfirmed = false,
                            HotelId = hotels[2].Id,
                            RoomId = rooms[2].Id,
                            ReservationGuests = new List<ReservationGuest>
                            {
                                new() { GuestId = guests[3].Id },
                                new() { GuestId = guests[4].Id },
                                new() { GuestId = guests[0].Id }
                            }
                        }
                    };

                    context.Reservations.AddRange(reservations);
                    context.SaveChanges();

                    // Emergency Contacts for each reservation:
                    var emergencyContacts = new List<EmergencyContact>
                    {
                        new() { FullName = "Carlos Ramírez", Phone = "3219876543", ReservationId = reservations[0].Id },
                        new() { FullName = "Ana Torres", Phone = "3229988776", ReservationId = reservations[1].Id },
                        new() { FullName = "Luis Méndez", Phone = "3005556677", ReservationId = reservations[2].Id }
                    };

                    context.EmergencyContacts.AddRange(emergencyContacts);
                    context.SaveChanges();
                }

                //if (!context.Reservations.Any())
                //{
                //    var room = context.Rooms.FirstOrDefault(); 
                //    var hotel = context.Hotels.FirstOrDefault(); 
                //    var guests = context.Guests.ToList();

                //    if (room != null && hotel != null && guests.Any())
                //    {
                //        var reservation = new Reservation
                //        {
                //            CheckInDate = new DateOnly(2025, 3, 10),
                //            CheckOutDate = new DateOnly(2025, 3, 15),
                //            NumberOfGuests = guests.Count,
                //            IsConfirmed = true,
                //            HotelId = hotel.Id,
                //            RoomId = room.Id,
                //            ReservationGuests = guests.Select(g => new ReservationGuest
                //            { GuestId = g.Id}).ToList()
                //        }; 

                //        context.Reservations.Add(reservation);
                //        context.SaveChanges();

                //        // Emergency contact:
                //        var emergencyContact = new EmergencyContact
                //        {
                //            FullName = "Carlos Ramírez",
                //            Phone = "3219876543",
                //            ReservationId = reservation.Id
                //        };

                //        context.EmergencyContacts.Add(emergencyContact);
                //        context.SaveChanges();
                //    }
                //}

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during database seeding: {ex.Message}");
        }
    }
}
