public class LocationService : ILocationService
{
    private readonly ILocationRepository _locationRepository;
    private readonly ICountryRepository _countryRepository;

    public LocationService(ILocationRepository locationRepository, ICountryRepository countryRepository)
    {
        _locationRepository = locationRepository;
        _countryRepository = countryRepository;
    }

    public async Task<List<LocationDto>> GetAllLocationsAsync()
    {
        var locations = await _locationRepository.GetAllAsync();
        return locations.Select(l => new LocationDto(l)).ToList();
    }

    public async Task<List<LocationDto>> GetLocationsByCountryAsync(long countryId)
    {
        var locations = await _locationRepository.GetByCountryAsync(countryId);
        return locations.Select(l => new LocationDto(l)).ToList();
    }

    public async Task<LocationDto> CreateLocationAsync(CreateLocationDto locationDto)
    {
        var countryExists = await _countryRepository.GetByIdAsync(locationDto.CountryId);
        if (countryExists == null)
            throw new KeyNotFoundException($"The specified country with ID {locationDto.CountryId} does not exist.");

        var existsByName = await _locationRepository.ExistsByCityNameAsync(locationDto.CityName, locationDto.CountryId);
        if (existsByName)
            throw new InvalidOperationException($"A city named '{locationDto.CityName}' already exists in the selected country.");

        var existsByCode = await _locationRepository.ExistsByCityCodeAsync(locationDto.CityCode);
        if (existsByCode)
            throw new InvalidOperationException($"A city with the code '{locationDto.CityCode}' already exists.");

        var location = new Location
        {
            CityName = locationDto.CityName.Trim(),
            CityCode = locationDto.CityCode.ToUpper(),
            CountryId = locationDto.CountryId
        };

        await _locationRepository.AddAsync(location);
        return new LocationDto(location);
    }

    public Task<bool> UpdateLocationAsync(long id, UpdateLocationDto locationDto)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteLocationAsync(long id)
    {
        throw new NotImplementedException();
    }
}
