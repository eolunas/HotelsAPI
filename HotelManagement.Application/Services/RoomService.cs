﻿public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IUserRepository _userRepository;


    public RoomService(IRoomRepository roomRepository, IUserRepository userRepository)
    {
        _roomRepository = roomRepository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<RoomDto>> GetFilteredRoomsAsync(bool? isAvailable, long? hotelId, long? maxNumberOfGuest)
    {
        var rooms = await _roomRepository.GetFilteredRoomsAsync(isAvailable, hotelId, maxNumberOfGuest);
        return rooms.Select(RoomDto.FromEntity);
    }

    public async Task<IEnumerable<RoomDto>> GetAllRoomsAsync()
    {
        var rooms = await _roomRepository.GetAllAsync();
        return rooms.Select(RoomDto.FromEntity);
    }

    public async Task<IEnumerable<RoomDto>> GetRoomsByHotelIdAsync(long hotelId)
    {
        var rooms = await _roomRepository.GetRoomsByHotelIdAsync(hotelId);
        return rooms.Select(RoomDto.FromEntity);
    }

    public async Task AddRoomAsync(CreateRoomDto createRoomDto, int userId)
    {
        var userExists = await _userRepository.ExistsAsync(userId);
        if (!userExists)
        {
            throw new KeyNotFoundException("The user does not exist.");
        }

        var room = new Room
        {
            RoomType = createRoomDto.RoomType,
            BasePrice = createRoomDto.BasePrice,
            Taxes = createRoomDto.Taxes,
            Location = createRoomDto.Location,
            MaxNumberOfGuest = createRoomDto.MaxNumberOfGuest,
            IsAvailable = createRoomDto.IsAvailable,
            CreatedByUserId = userId
        };
        await _roomRepository.AddAsync(room);
    }

    public async Task UpdateRoomAsync(UpdateRoomDto roomDto)
    {
        var room = await _roomRepository.GetByIdAsync(roomDto.Id);
        if (room == null) throw new KeyNotFoundException("Room not found");

        room.RoomType = roomDto.RoomType;
        room.BasePrice = roomDto.BasePrice;
        room.Taxes = roomDto.Taxes;
        room.Location = roomDto.Location;
        room.MaxNumberOfGuest = roomDto.MaxNumberOfGuest;
        room.IsAvailable = roomDto.IsAvailable;

        await _roomRepository.UpdateAsync(room);
    }

    public async Task ToggleRoomStatusAsync(long roomId, bool isAvailable)
    {
        var room = await _roomRepository.GetByIdAsync(roomId);
        if (room == null)
        {
            throw new KeyNotFoundException("Room not found.");
        }

        room.IsAvailable = isAvailable;

        await _roomRepository.UpdateAsync(room);
    }

    public async Task DeleteRoomAsync(long id)
    {
        await _roomRepository.DeleteAsync(id);
    }
}
