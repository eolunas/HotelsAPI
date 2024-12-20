/// <summary>
/// Interface for hotel search functionality.
/// </summary>
public interface IHotelSearchService
{
    /// <summary>
    /// Searches hotels based on the specified criteria.
    /// </summary>
    /// <param name="criteria">Search criteria including city, dates, and number of guests.</param>
    /// <returns>A list of hotels matching the search criteria.</returns>
    Task<IEnumerable<HotelSearchResultDto>> SearchHotelsAsync(HotelSearchCriteriaDto criteria);
}
