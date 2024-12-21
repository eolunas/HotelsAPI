public interface IReservationService
{
    Task<IEnumerable<ReservationDto>> GetReservationsByRoomIdAsync(long roomId);
    Task AddReservationAsync(ReservationDto reservationDto);
    Task DeleteReservationAsync(long id);
}
