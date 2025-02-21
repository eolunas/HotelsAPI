public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IRepository<Hotel> _hotelRepository;
    private readonly IGuestRepository _guestRepository;
    private readonly IEmergencyContactRepository _emergencyContactRepository;
    private readonly IEmailService _emailService;

    public ReservationService(
        IReservationRepository reservationRepository,
        IRepository<Hotel> hotelRepository,
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
        var emergencyContact = await _emergencyContactRepository.GetByReservationIdAsync(reservation.Id);

        if (reservation.Id == 0)
            throw new KeyNotFoundException($"Reservation with ID {id} not found.");

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
            Guest = new GuestDto
            {
                FullName = reservation.Guest.FullName,
                BirthDate = reservation.Guest.BirthDate,
                Gender = reservation.Guest.Gender,
                DocumentType = reservation.Guest.DocumentType,
                DocumentNumber = reservation.Guest.DocumentNumber,
                Email = reservation.Guest.Email,
                Phone = reservation.Guest.Phone
            },
            EmergencyContact = new EmergencyContactDto
            {
                FullName = emergencyContact.FullName,
                Phone = emergencyContact.Phone
            },
            Hotel = new HotelDetailDto
            {
                Id = reservation.Hotel.Id,
                Name = reservation.Hotel.Name,
                Location = reservation.Hotel.Location, 
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
            // Create or find a Guest:
            var guest = new Guest
            {
                FullName = reservationDto.Guest.FullName,
                BirthDate = reservationDto.Guest.BirthDate,
                Gender = reservationDto.Guest.Gender,
                Email = reservationDto.Guest.Email,
                Phone = reservationDto.Guest.Phone,
                DocumentType = reservationDto.Guest.DocumentType,
                DocumentNumber = reservationDto.Guest.DocumentNumber
            };

            guest = await _guestRepository.AddOrUpdateAsync(guest);

            // Create the reservation:
            var reservation = new Reservation
            {
                RoomId = reservationDto.RoomId,
                CheckInDate = reservationDto.CheckInDate,
                CheckOutDate = reservationDto.CheckOutDate,
                NumberOfGuests = reservationDto.NumberOfGuests,
                IsConfirmed = true,
                HotelId = room.HotelId ?? 0,
                GuestId = guest.Id
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

            // Send confirmation via email:
            await _emailService.SendReservationConfirmationAsync(guest, reservation);
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
