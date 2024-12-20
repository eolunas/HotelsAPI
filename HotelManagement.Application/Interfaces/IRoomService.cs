public interface IRoomService
{
    Task<IEnumerable<RoomDto>> GetRoomsByHotelIdAsync(Guid hotelId);
    Task AddRoomAsync(RoomDto roomDto);
    Task UpdateRoomAsync(RoomDto roomDto);
    Task DeleteRoomAsync(Guid id);
}
