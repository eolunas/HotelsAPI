public interface IRoomService
{
    Task<IEnumerable<RoomDto>> GetAllRoomsAsync();
    Task<IEnumerable<RoomDto>> GetRoomsByHotelIdAsync(long hotelId);
    Task AddRoomAsync(CreateRoomDto createRoomDto, int userId);
    Task UpdateRoomAsync(UpdateRoomDto roomDto);
    Task ToggleRoomStatusAsync(long roomId, bool isAvailable);
    Task DeleteRoomAsync(long id);
}
