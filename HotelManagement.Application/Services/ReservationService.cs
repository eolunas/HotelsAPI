public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IRoomRepository _roomRepository;

    public ReservationService(IReservationRepository reservationRepository, IRoomRepository roomRepository)
    {
        _reservationRepository = reservationRepository;
        _roomRepository = roomRepository;
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
