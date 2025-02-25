public interface ILocationRepository : IRepository<Location>
{
    Task<List<Location>> GetByCountryAsync(long countryId);
    Task<bool> ExistsAsync(long locationId);
    Task<bool> ExistsByCityNameAsync(string cityName, long countryId);
    Task<bool> ExistsByCityCodeAsync(string cityCode);
}
