public class HotelSearchService : IHotelSearchService
{
    private readonly IRepository<Hotel> _hotelRepository;
    private readonly IRepository<Room> _roomRepository;
    private readonly IRepository<Reservation> _reservationRepository;

    public HotelSearchService(IRepository<Hotel> hotelRepository,
                               IRepository<Room> roomRepository,
                               IRepository<Reservation> reservationRepository)
    {
        _hotelRepository = hotelRepository;
        _roomRepository = roomRepository;
        _reservationRepository = reservationRepository;
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
