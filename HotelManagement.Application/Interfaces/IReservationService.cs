public interface IReservationService
{
    Task<IEnumerable<ReservationDto>> GetAllReservationsAsync();
    Task<ReservationDetailDto> GetReservationDetailAsync(long id);
    Task<IEnumerable<ReservationDto>> GetReservationsByRoomIdAsync(long roomId);
    Task CreateReservationAsync(CreateReservationDto reservationDto);
    Task DeleteReservationAsync(long id);
}
