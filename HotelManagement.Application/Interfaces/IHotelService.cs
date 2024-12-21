public interface IHotelService
{
    Task<IEnumerable<HotelDto>> GetAllHotelsAsync();
    Task<HotelDto> GetHotelByIdAsync(Guid id);
    Task AddHotelAsync(HotelDto hotelDto);
    Task UpdateHotelAsync(Guid hotelId, UpdateHotelDto updateHotelDto);
    Task ToggleHotelStatusAsync(Guid hotelId, bool isEnabled);
    Task AssignRoomsToHotelAsync(AssignRoomsToHotelDto assignRoomsDto);

    /// <summary>
    /// Searches hotels based on the specified criteria.
    /// </summary>
    /// <param name="criteria">Search criteria including city, dates, and number of guests.</param>
    /// <returns>A list of hotels matching the search criteria.</returns>
    Task<IEnumerable<HotelSearchResultDto>> SearchHotelsAsync(HotelSearchCriteriaDto criteria);
}
