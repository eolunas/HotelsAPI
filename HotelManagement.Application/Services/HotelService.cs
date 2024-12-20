public class HotelService : IHotelService
{
    private readonly IRepository<Hotel> _hotelRepository;

    public HotelService(IRepository<Hotel> hotelRepository)
    {
        _hotelRepository = hotelRepository;
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
}
