public interface IHotelRepository : IRepository<Hotel>
{
    Task<List<Hotel>> GetFilteredHotelsAsync(bool? isEnabled, long? locationId, long? createdByUserId);
    Task<bool> ExistsInLocationAsync(string normalizedName, int locationId);
}
