public interface ILocationService
{
    Task<List<LocationDto>> GetAllLocationsAsync();
    Task<List<LocationDto>> GetLocationsByCountryAsync(long countryId);
    Task<LocationDto> CreateLocationAsync(CreateLocationDto locationDto);
    Task<bool> UpdateLocationAsync(long id, UpdateLocationDto locationDto);
    Task<bool> DeleteLocationAsync(long id);
}
