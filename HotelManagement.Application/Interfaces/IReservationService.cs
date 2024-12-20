public interface IReservationService
{
    Task<IEnumerable<ReservationDto>> GetReservationsByRoomIdAsync(Guid roomId);
    Task AddReservationAsync(ReservationDto reservationDto);
    Task DeleteReservationAsync(Guid id);
}
