public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;

    public RoomService(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task<IEnumerable<RoomDto>> GetRoomsByHotelIdAsync(Guid hotelId)
    {
        var rooms = await _roomRepository.GetRoomsByHotelIdAsync(hotelId);
        return rooms.Select(r => new RoomDto
        {
            Id = r.Id,
            RoomType = r.RoomType,
            BasePrice = r.BasePrice,
            Taxes = r.Taxes,
            Location = r.Location,
            IsAvailable = r.IsAvailable,
            HotelId = r.HotelId
        });
    }

    public async Task AddRoomAsync(RoomDto roomDto)
    {
        var room = new Room
        {
            Id = Guid.NewGuid(),
            RoomType = roomDto.RoomType,
            BasePrice = roomDto.BasePrice,
            Taxes = roomDto.Taxes,
            Location = roomDto.Location,
            IsAvailable = roomDto.IsAvailable,
            HotelId = roomDto.HotelId
        };
        await _roomRepository.AddAsync(room);
    }

    public async Task UpdateRoomAsync(RoomDto roomDto)
    {
        var room = await _roomRepository.GetByIdAsync(roomDto.Id);
        if (room == null) throw new KeyNotFoundException("Room not found");

        room.RoomType = roomDto.RoomType;
        room.BasePrice = roomDto.BasePrice;
        room.Taxes = roomDto.Taxes;
        room.Location = roomDto.Location;
        room.IsAvailable = roomDto.IsAvailable;

        await _roomRepository.UpdateAsync(room);
    }

    public async Task DeleteRoomAsync(Guid id)
    {
        await _roomRepository.DeleteAsync(id);
    }
}
