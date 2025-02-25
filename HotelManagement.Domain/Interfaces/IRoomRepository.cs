public interface IRoomRepository : IRepository<Room>
{
    Task<IEnumerable<Room>> GetFilteredRoomsAsync(bool? isAvailable, long? hotelId, long? maxNumberOfGuest);
    Task<IEnumerable<Room>> GetRoomsByHotelIdAsync(long hotelId);
    Task UpdateAsyncRange(IEnumerable<Room> rooms);
}
