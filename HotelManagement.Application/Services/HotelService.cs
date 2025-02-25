public class HotelService : IHotelService
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILocationRepository _locationRepository;

    public HotelService(IHotelRepository hotelRepository,
                        IRoomRepository roomRepository,
                        IReservationRepository reservationRepository,
                        IUserRepository userRepository,
                        ILocationRepository locationRepository)
    {
        _hotelRepository = hotelRepository;
        _roomRepository = roomRepository;
        _reservationRepository = reservationRepository;
        _userRepository = userRepository;
        _locationRepository = locationRepository;
    }

    public async Task<IEnumerable<HotelDto>> GetFilteredHotelsAsync(bool? isEnabled, long? locationId, long? createdByUserId)
    {
        var hotels = await _hotelRepository.GetFilteredHotelsAsync(isEnabled, locationId, createdByUserId);
        return hotels.Select(HotelDto.FromEntity);
    }

    public async Task<IEnumerable<HotelDto>> GetAllHotelsAsync()
    {
        var hotels = await _hotelRepository.GetAllAsync();
        return hotels.Select(HotelDto.FromEntity);
    }

    public async Task<HotelDto> GetHotelByIdAsync(long id)
    {
        var hotel = await _hotelRepository.GetByIdAsync(id);
        if (hotel.Id == 0) throw new KeyNotFoundException("Hotel not found");
        return HotelDto.FromEntity(hotel);
    }

    public async Task AddHotelAsync(CreateHotelDto createHotelDto, int userId)
    {
        var userExists = await _userRepository.ExistsAsync(userId);
        if (!userExists)
            throw new KeyNotFoundException("The user ID provided does not exist.");

        var locationExists = await _locationRepository.ExistsAsync(createHotelDto.LocationId);
        if (!locationExists)
            throw new KeyNotFoundException($"Invalid LocationId: {createHotelDto.LocationId}. The specified city does not exist.");

        var normalizedName = createHotelDto.Name.Trim().ToLower();
        var hotelExists = await _hotelRepository.ExistsInLocationAsync(normalizedName, createHotelDto.LocationId);
        if (hotelExists)
            throw new ValidationException($"A hotel with the name '{createHotelDto.Name}' already exists in this location.");

        var newHotel = new Hotel
        {
            Name = createHotelDto.Name.Trim(),
            LocationId = createHotelDto.LocationId,
            IsEnabled = createHotelDto.IsEnabled,
            CreatedByUserId = userId
        };

        await _hotelRepository.AddAsync(newHotel);
    }

    public async Task UpdateHotelAsync(UpdateHotelDto updateHotelDto)
    {
        var hotel = await _hotelRepository.GetByIdAsync(updateHotelDto.Id) ?? throw new KeyNotFoundException("Hotel not found.");

        var locationExists = await _locationRepository.ExistsAsync(updateHotelDto.LocationId);
        if (!locationExists)
        {
            throw new KeyNotFoundException("Invalid LocationId. The city does not exist.");
        }

        hotel.Name = updateHotelDto.Name;
        hotel.LocationId = updateHotelDto.LocationId;
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
        
        // Validate criteria inputs:
        HotelSearchCriteriaValidator.Validate(criteria);

        // Filter hotels in the city with available rooms and max number of guest according to criteria:
        var hotels = await _hotelRepository.GetAllAsync();
        var filteredHotels = hotels.Where(h => 
            h.LocationId.Equals(criteria.LocationId) &&
            h.IsEnabled &&
            h.Rooms != null && h.Rooms.Any(r => r.IsAvailable))
            .Select(h => new Hotel
            {
                Id = h.Id,
                Name = h.Name,
                Location = h.Location,
                Rooms = h.Rooms.Where(r => r.IsAvailable && r.MaxNumberOfGuest >= criteria.NumberOfGuests).ToList()
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
                    MaxNumberOfGuest = room.MaxNumberOfGuest,
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
                    Location = hotel.Location.CityName,
                    AvailableRoomTypes = availableRooms
                });
        }

        return results;
    }
}
