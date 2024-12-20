public interface IRoomRepository : IRepository<Room>
{
    Task<IEnumerable<Room>> GetRoomsByHotelIdAsync(Guid hotelId);
    Task UpdateAsyncRange(IEnumerable<Room> rooms);
}
