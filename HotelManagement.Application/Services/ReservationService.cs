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
        try
        {
            // Check the room:
            var room = await _roomRepository.GetByIdAsync(reservationDto.RoomId);
            if (room == null || !room.IsAvailable)
            {
                throw new InvalidOperationException("The selected room is not available.");
            }

            // Create or find a Guest:
            var guest = new Guest
            {
                FullName = reservationDto.Guest.FullName,
                BirthDate = reservationDto.Guest.BirthDate,
                Gender =reservationDto.Guest.Gender,
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

            // Crear contacto de emergencia (si está presente en el DTO)
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

            // Actualizar estado de la habitación
            room.IsAvailable = false;
            await _roomRepository.UpdateAsync(room);

            // Enviar notificación por correo
            await _emailService.SendReservationConfirmationAsync(guest.Email, reservation.Id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error: {ex.Message}");
        }
    }


    // Deprecate 
    public async Task AddReservationAsync(ReservationDto reservationDto)
    {
        var room = await _roomRepository.GetByIdAsync(reservationDto.RoomId);
        if (room == null || !room.IsAvailable)
            throw new InvalidOperationException("Room is not available");

        var reservation = new Reservation
        {
            CheckInDate = reservationDto.CheckInDate,
            CheckOutDate = reservationDto.CheckOutDate,
            NumberOfGuests = reservationDto.NumberOfGuests,
            RoomId = reservationDto.RoomId,
            IsConfirmed = reservationDto.IsConfirmed
        };

        await _reservationRepository.AddAsync(reservation);
    }

    public async Task DeleteReservationAsync(long id)
    {
        await _reservationRepository.DeleteAsync(id);
    }
}
