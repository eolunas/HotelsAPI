public class HotelService : IHotelService
{
    private readonly IRepository<Hotel> _hotelRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IReservationRepository _reservationRepository;

    public HotelService(IRepository<Hotel> hotelRepository,
                        IRoomRepository roomRepository,
                        IReservationRepository reservationRepository)
    {
        _hotelRepository = hotelRepository;
        _roomRepository = roomRepository;
        _reservationRepository = reservationRepository;
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
            IsEnabled = h.IsEnabled
        });
    }

    public async Task<HotelDto> GetHotelByIdAsync(Guid id)
    {
        var hotel = await _hotelRepository.GetByIdAsync(id);
        if (hotel == null) throw new KeyNotFoundException("Hotel not found");
        return new HotelDto
        {
            Id = hotel.Id,
            Name = hotel.Name,
            Location = hotel.Location,
            BasePrice = hotel.BasePrice,
            IsEnabled = hotel.IsEnabled
        };
    }

    public async Task AddHotelAsync(HotelDto hotelDto)
    {
        var hotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = hotelDto.Name,
            Location = hotelDto.Location,
            BasePrice = hotelDto.BasePrice,
            IsEnabled = hotelDto.IsEnabled
        };
        await _hotelRepository.AddAsync(hotel);
    }

    public async Task UpdateHotelAsync(Guid hotelId, UpdateHotelDto updateHotelDto)
    {
        var hotel = await _hotelRepository.GetByIdAsync(hotelId);

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

    public async Task AssignRoomsToHotelAsync(AssignRoomsToHotelDto assignRoomsDto)
    {
        var hotel = await _hotelRepository.GetByIdAsync(assignRoomsDto.HotelId);
        if (hotel == null)
        {
            throw new KeyNotFoundException("Hotel not found.");
        }

        var rooms = await _roomRepository.GetAllAsync();
        var roomsToAssign = rooms.Where(r => assignRoomsDto.RoomIds.Contains(r.Id)).ToList();

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
        // Filtrar hoteles en la ciudad solicitada
        var hotels = await _hotelRepository.GetAllAsync();
        var filteredHotels = hotels.Where(h => h.Location.Equals(criteria.City, StringComparison.OrdinalIgnoreCase));

        var results = new List<HotelSearchResultDto>();

        foreach (var hotel in filteredHotels)
        {
            // Obtener habitaciones disponibles
            var rooms = await _roomRepository.GetAllAsync();
            var availableRooms = rooms
                .Where(r => r.HotelId == hotel.Id && r.IsAvailable)
                .ToList();

            // Verificar disponibilidad según reservas existentes
            foreach (var room in availableRooms.ToList())
            {
                var reservations = await _reservationRepository.GetAllAsync();
                if (reservations.Any(r => r.RoomId == room.Id &&
                                          r.CheckInDate < criteria.CheckOutDate &&
                                          r.CheckOutDate > criteria.CheckInDate))
                {
                    availableRooms.Remove(room);
                }
            }

            // Verificar capacidad
            var suitableRooms = availableRooms
                .Where(r => r.RoomType.Contains("Double") || r.RoomType.Contains("Family")) // Ejemplo de capacidad
                .Select(r => r.RoomType)
                .Distinct();

            if (suitableRooms.Any())
            {
                results.Add(new HotelSearchResultDto
                {
                    HotelId = hotel.Id,
                    HotelName = hotel.Name,
                    Location = hotel.Location,
                    AvailableRoomTypes = suitableRooms
                });
            }
        }

        return results;
    }
}
