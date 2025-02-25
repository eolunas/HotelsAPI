public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IGuestRepository _guestRepository;
    private readonly IEmergencyContactRepository _emergencyContactRepository;
    private readonly IEmailService _emailService;

    public ReservationService(
        IReservationRepository reservationRepository,
        IHotelRepository hotelRepository,
        IRoomRepository roomRepository,
        IGuestRepository guestRepository,
        IEmergencyContactRepository emergencyContactRepository,
        IEmailService emailService
        )
    {
        _reservationRepository = reservationRepository;
        _hotelRepository = hotelRepository;
        _roomRepository = roomRepository;
        _guestRepository = guestRepository;
        _emailService = emailService;
        _emergencyContactRepository = emergencyContactRepository;
    }

    public async Task<IEnumerable<ReservationDto>> GetAllReservationsAsync()
    {
        var reservations = await _reservationRepository.GetAllAsync();
        return reservations.Select(r => new ReservationDto
        {
            Id = r.Id,
            CheckInDate = r.CheckInDate,
            CheckOutDate = r.CheckOutDate,
            NumberOfGuests = r.NumberOfGuests,
            HotelId = r.HotelId,
            RoomId = r.RoomId,
            IsConfirmed = r.IsConfirmed,
            Nights = r.Nights,
        });
    }

    public async Task<ReservationDetailDto> GetReservationDetailAsync(long id)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id);
        
        if (reservation.Id == 0)
            throw new KeyNotFoundException($"Reservation with ID {id} not found.");
        
        var emergencyContact = await _emergencyContactRepository.GetByReservationIdAsync(reservation.Id);

        // TotalCost calculation: 
        var totalCost = reservation.Nights * (reservation.Room.BasePrice + reservation.Room.Taxes);

        return new ReservationDetailDto
        {
            Id = reservation.Id,
            CheckInDate = reservation.CheckInDate,
            CheckOutDate = reservation.CheckOutDate,
            NumberOfGuests = reservation.NumberOfGuests,
            Nights = reservation.Nights,
            TotalCost = totalCost,
            Guests = reservation.ReservationGuests
            .Select(rg => new GuestDto
            {
                FullName = rg.Guest.FullName,
                BirthDate = rg.Guest.BirthDate,
                Gender = rg.Guest.Gender,
                DocumentType = rg.Guest.DocumentType,
                DocumentNumber = rg.Guest.DocumentNumber,
                Email = rg.Guest.Email,
                Phone = rg.Guest.Phone
            }).ToList(),
            EmergencyContact = new EmergencyContactDto
            {
                FullName = emergencyContact.FullName,
                Phone = emergencyContact.Phone
            } ?? new EmergencyContactDto { },
            Hotel = new HotelDetailDto
            {
                Id = reservation.Hotel.Id,
                Name = reservation.Hotel.Name,
                Location = reservation.Hotel.Location.CityName, 
            },
            Room = new RoomDetailDto
            {
                Id = reservation.Room.Id,
                RoomType = reservation.Room.RoomType,
                BasePrice = reservation.Room.BasePrice,
                Taxes = reservation.Room.Taxes,
                Location = reservation.Room.Location
            }
        };
    }

    public async Task<IEnumerable<ReservationDto>> GetReservationsByRoomIdAsync(long roomId)
    {
        var reservations = await _reservationRepository.GetReservationsByRoomIdAsync(roomId);
        return reservations.Select(r => new ReservationDto
        {
            Id = r.Id,
            CheckInDate = r.CheckInDate,
            CheckOutDate = r.CheckOutDate,
            NumberOfGuests = r.NumberOfGuests,
            HotelId = r.HotelId,
            RoomId = r.RoomId,
            IsConfirmed = r.IsConfirmed,
            Nights = r.Nights,
        });
    }

    public async Task CreateReservationAsync(CreateReservationDto reservationDto)
    {
        // Validate de input criteria:
        ReservationValidator.Validate(reservationDto);

        // Check the room and hotel:
        var room = await _roomRepository.GetByIdAsync(reservationDto.RoomId);
        if (room.Hotel == null || !room.Hotel.IsEnabled)
        {
            throw new InvalidOperationException("The selected hotel is not enable.");
        }

        if (room == null || !room.IsAvailable)
        {
            throw new InvalidOperationException("The selected room is not available.");
        }

        if (room.MaxNumberOfGuest < reservationDto.NumberOfGuests)
        {
            throw new InvalidOperationException("The selected room has not enough capacity for the number of guests.");
        }

        // Verify the availability of the room: 
        var roomReservations = await GetReservationsByRoomIdAsync(reservationDto.RoomId);

        bool isRoomReserved = roomReservations.Any(existing =>
            reservationDto.CheckInDate < existing.CheckOutDate &&
            reservationDto.CheckOutDate > existing.CheckInDate
        );

        if (isRoomReserved)
        {
            throw new InvalidOperationException("The room is already reserved for the selected dates.");
        }

        try
        {
            // Create/update the Guests:
            var ReservationGuests = new List<ReservationGuest>();
            foreach (var guest in reservationDto.Guests)
            {
                var guestAdded = await _guestRepository.AddOrUpdateAsync(new Guest
                {
                    FullName = guest.FullName,
                    BirthDate = guest.BirthDate,
                    Gender = guest.Gender,
                    Email = guest.Email,
                    Phone = guest.Phone,
                    DocumentType = guest.DocumentType,
                    DocumentNumber = guest.DocumentNumber
                });

                ReservationGuests.Add(new ReservationGuest { GuestId = guestAdded .Id });
            }

            // Create the reservation:
            var reservation = new Reservation
            {
                RoomId = reservationDto.RoomId,
                CheckInDate = reservationDto.CheckInDate,
                CheckOutDate = reservationDto.CheckOutDate,
                NumberOfGuests = reservationDto.NumberOfGuests,
                IsConfirmed = true,
                HotelId = room.HotelId ?? 0,
                ReservationGuests = ReservationGuests
            };

            await _reservationRepository.AddAsync(reservation);

            // Create emergency contact:
            if (reservationDto.EmergencyContact != null)
            {
                var emergencyContact = new EmergencyContact
                {
                    FullName = reservationDto.EmergencyContact.FullName,
                    Phone = reservationDto.EmergencyContact.Phone,
                    ReservationId = reservation.Id
                };

                await _emergencyContactRepository.AddAsync(emergencyContact);
            }

            // Send confirmation via email: [Every Guest]
            await _emailService.SendReservationConfirmationAsync(reservationDto.Guests, reservation);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error: {ex.Message}");
        }
    }

    public async Task DeleteReservationAsync(long id)
    {
        await _reservationRepository.DeleteAsync(id);
    }
}
