public interface IReservationRepository : IRepository<Reservation>
{
    Task<IEnumerable<Reservation>> GetReservationsByRoomIdAsync(long roomId);
    Task<IEnumerable<Reservation>> GetReservationsByDateRangeAsync(DateOnly startDate, DateOnly endDate);
}
