public interface IHotelService
{
    Task<IEnumerable<HotelDto>> GetAllHotelsAsync();
    Task<IEnumerable<HotelDto>> GetFilteredHotelsAsync(bool? isEnabled, long? locationId, long? createdByUserId);
    Task<HotelDto> GetHotelByIdAsync(long id);
    Task AddHotelAsync(CreateHotelDto hotelDto, int userId);
    Task UpdateHotelAsync(UpdateHotelDto updateHotelDto);
    Task DeleteHotelAsync(long id);
    Task ToggleHotelStatusAsync(long hotelId, bool isEnabled);
    Task AssignRoomsToHotelAsync(AssignRoomsToHotelDto assignRoomsDto);

    /// <summary>
    /// Searches hotels based on the specified criteria.
    /// </summary>
    /// <param name="criteria">Search criteria including city, dates, and number of guests.</param>
    /// <returns>A list of hotels matching the search criteria.</returns>
    Task<IEnumerable<HotelSearchResultDto>> SearchHotelsAsync(HotelSearchCriteriaDto criteria);
}
