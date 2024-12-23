public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IGuestRepository _guestRepository;
    private readonly IEmergencyContactRepository _emergencyContactRepository;
    private readonly IEmailService _emailService;

    public ReservationService(
        IReservationRepository reservationRepository,
        IRoomRepository roomRepository,
        IGuestRepository guestRepository,
        IEmergencyContactRepository emergencyContactRepository,
        IEmailService emailService
        )
    {
        _reservationRepository = reservationRepository;
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
            RoomId = r.RoomId,
            IsConfirmed = r.IsConfirmed
        });
    }

    public async Task<ReservationDetailDto> GetReservationDetailAsync(long id)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id);
        var emergencyContact = await _emergencyContactRepository.GetByReservationIdAsync(reservation.Id);

        if (reservation.Id == 0)
            throw new KeyNotFoundException($"Reservation with ID {id} not found.");

        return new ReservationDetailDto
        {
            Id = reservation.Id,
            CheckInDate = reservation.CheckInDate,
            CheckOutDate = reservation.CheckOutDate,
            NumberOfGuests = reservation.NumberOfGuests,
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
            Room = new RoomDto
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
            RoomId = r.RoomId,
            IsConfirmed = r.IsConfirmed
        });
    }

    public async Task CreateReservationAsync(CreateReservationDto reservationDto)
    {
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
            await _emailService.SendReservationConfirmationAsync(guest.Email, reservation.Id);
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
