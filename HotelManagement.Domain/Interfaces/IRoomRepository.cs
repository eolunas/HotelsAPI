public interface IRoomRepository : IRepository<Room>
{
    Task<IEnumerable<Room>> GetRoomsByHotelIdAsync(long hotelId);
    Task UpdateAsyncRange(IEnumerable<Room> rooms);
}
