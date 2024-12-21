public interface IRoomService
{
    Task<IEnumerable<RoomDto>> GetRoomsByHotelIdAsync(long hotelId);
    Task AddRoomAsync(RoomDto roomDto);
    Task UpdateRoomAsync(RoomDto roomDto);
    Task ToggleRoomStatusAsync(long roomId, bool isAvailable);
    Task DeleteRoomAsync(long id);
}
