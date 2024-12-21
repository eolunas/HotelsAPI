public interface IRoomService
{
    Task<IEnumerable<RoomDto>> GetRoomsByHotelIdAsync(Guid hotelId);
    Task AddRoomAsync(RoomDto roomDto);
    Task UpdateRoomAsync(RoomDto roomDto);
    Task ToggleRoomStatusAsync(Guid hotelId, bool isEnabled);
    Task DeleteRoomAsync(Guid id);
}
