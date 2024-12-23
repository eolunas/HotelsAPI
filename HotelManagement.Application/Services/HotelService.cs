public class HotelService : IHotelService
{
    private readonly IRepository<Hotel> _hotelRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IUserRepository _userRepository;

    public HotelService(IRepository<Hotel> hotelRepository,
                        IRoomRepository roomRepository,
                        IReservationRepository reservationRepository,
                        IUserRepository userRepository)
    {
        _hotelRepository = hotelRepository;
        _roomRepository = roomRepository;
        _reservationRepository = reservationRepository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<HotelDto>> GetAllHotelsAsync()
    {
        var hotels = await _hotelRepository.GetAllAsync();
        return hotels.Select(h => new HotelDto
        {
            Id = h.Id,
            Name = h.Name,
            Location = h.Location,
            BasePrice = h.BasePrice,
            IsEnabled = h.IsEnabled,
            CreatedByUserId = h.CreatedByUserId,
            Rooms = h.Rooms?.Select(r => new RoomDto
            {
                Id = r.Id,
                RoomType = r.RoomType,
                Taxes = r.Taxes,
                BasePrice = r.BasePrice,
                Location = r.Location,
                IsAvailable = r.IsAvailable,
                HotelId = r.HotelId
            }).ToList() ?? []
        });
    }

    public async Task<HotelDto> GetHotelByIdAsync(long id)
    {
        var hotel = await _hotelRepository.GetByIdAsync(id);
        if (hotel.Id == 0) throw new KeyNotFoundException("Hotel not found");
        return new HotelDto
        {
            Id = hotel.Id,
            Name = hotel.Name,
            Location = hotel.Location,
            BasePrice = hotel.BasePrice,
            IsEnabled = hotel.IsEnabled,
            Rooms = hotel.Rooms?.Select(r => new RoomDto
            {
                Id = r.Id,
                RoomType = r.RoomType,
                Taxes = r.Taxes,
                BasePrice = r.BasePrice,
                Location = r.Location,
                IsAvailable = r.IsAvailable,
                HotelId = r.HotelId
            }).ToList() ?? []
        };
    }

    public async Task AddHotelAsync(CreateHotelDto createHotelDto, int userId)
    {
        var userExists = await _userRepository.ExistsAsync(userId);
        if (!userExists)
        {
            throw new KeyNotFoundException("The user does not exist.");
        }

        var newHotel = new Hotel
        {
            Name = createHotelDto.Name,
            Location = createHotelDto.Location,
            BasePrice = createHotelDto.BasePrice,
            IsEnabled = createHotelDto.IsEnabled,
            CreatedByUserId = userId
        };

        await _hotelRepository.AddAsync(newHotel);
    }

    public async Task UpdateHotelAsync(UpdateHotelDto updateHotelDto)
    {
        var hotel = await _hotelRepository.GetByIdAsync(updateHotelDto.Id);

        if (hotel == null)
        {
            throw new KeyNotFoundException("Hotel not found.");
        }

        hotel.Name = updateHotelDto.Name;
        hotel.Location = updateHotelDto.Location;
        hotel.BasePrice = updateHotelDto.BasePrice;
        hotel.IsEnabled = updateHotelDto.IsEnabled;

        await _hotelRepository.UpdateAsync(hotel);
    }

    public async Task DeleteHotelAsync(long id)
    {
        await _hotelRepository.DeleteAsync(id);
    }

    public async Task ToggleHotelStatusAsync(long hotelId, bool isEnabled)
    {
        var hotel = await _hotelRepository.GetByIdAsync(hotelId);
        if (hotel == null)
        {
            throw new KeyNotFoundException("Hotel not found.");
        }

        hotel.IsEnabled = isEnabled;

        await _hotelRepository.UpdateAsync(hotel);
    }

    public async Task AssignRoomsToHotelAsync(AssignRoomsToHotelDto assignRoomsDto)
    {
        var hotel = await _hotelRepository.GetByIdAsync(assignRoomsDto.HotelId);
        if (hotel.Id == 0)
        {
            throw new KeyNotFoundException("Hotel not found.");
        }

        var rooms = await _roomRepository.GetAllAsync();
        var roomsToAssign = rooms.Where(r => assignRoomsDto.RoomIds.Contains(r.Id)).ToList();

        if (roomsToAssign.Count() == 0)
        {
            throw new InvalidOperationException($"There are not rooms to assign.");
        }

        foreach (var room in roomsToAssign)
        {
            if (room.HotelId.HasValue)
            {
                throw new InvalidOperationException($"Room {room.Id} is already assigned to another hotel.");
            }

            room.HotelId = hotel.Id;
        }

        await _roomRepository.UpdateAsyncRange(roomsToAssign);
    }

    public async Task<IEnumerable<HotelSearchResultDto>> SearchHotelsAsync(HotelSearchCriteriaDto criteria)
    {
        var results = new List<HotelSearchResultDto>();
        
        // Validate input dates:
        HotelSearchCriteriaValidator.Validate(criteria);

        // Filter hotel in the city with available rooms:
        var hotels = await _hotelRepository.GetAllAsync();
        var filteredHotels = hotels.Where(h => 
            h.Location?.Trim().Equals(criteria.City.Trim(), StringComparison.OrdinalIgnoreCase) == true &&
            h.IsEnabled &&
            h.Rooms != null && h.Rooms.Any(r => r.IsAvailable))
            .Select(h => new Hotel
            {
                Id = h.Id,
                Name = h.Name,
                Location = h.Location,
                Rooms = h.Rooms.Where(r => r.IsAvailable).ToList()
            }).ToList();

        // Get the reservation into dates interval:
        var reservationInDates = _reservationRepository.GetReservationsByDateRangeAsync(criteria.CheckInDate, criteria.CheckOutDate).Result;

        // Verified dates for reservation:
        foreach (var hotel in filteredHotels)
        {
            var availableRooms = new List<RoomDto>();
            foreach (var room in hotel.Rooms)
            {
                // Check if room are reserved:
                var reserved = reservationInDates.Where(r => r.RoomId == room.Id).Any();
                if (!reserved) availableRooms.Add(new RoomDto
                {
                    Id = room.Id,
                    RoomType = room.RoomType,
                    BasePrice = room.BasePrice,
                    Taxes = room.Taxes,
                    Location = room.Location,
                    IsAvailable = room.IsAvailable,
                    HotelId = room.HotelId
                });
            }

            // Add the available rooms into hotels response:
            if(availableRooms.Count > 0) 
                results.Add(new HotelSearchResultDto
                {
                    HotelId = hotel.Id,
                    HotelName = hotel.Name,
                    Location = hotel.Location,
                    AvailableRoomTypes = availableRooms
                });
        }

        return results;
    }
}
