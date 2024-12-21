public interface IReservationService
{
    Task<IEnumerable<ReservationDto>> GetReservationsByRoomIdAsync(long roomId);
    Task AddReservationAsync(ReservationDto reservationDto);
    Task CreateReservationAsync(CreateReservationDto reservationDto);
    Task DeleteReservationAsync(long id);
}
