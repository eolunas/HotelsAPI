using Microsoft.EntityFrameworkCore;

public class LocationRepository : ILocationRepository
{
    private readonly ApplicationDbContext _context;

    public LocationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Location> GetByIdAsync(long id)
    {
        return await _context.Locations.FindAsync(id);
    }

    public async Task<IEnumerable<Location>> GetAllAsync()
    {
        return await _context.Locations
            .Include(l => l.Country)
            .ToListAsync();
    }

    public async Task AddAsync(Location entity)
    {
        await _context.Locations.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Location entity)
    {
        _context.Locations.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location == null) return;
        _context.Locations.Remove(location);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Location>> GetByCountryAsync(long countryId)
    {
        return await _context.Locations
            .Where(l => l.CountryId == countryId)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(long locationId)
    {
        return await _context.Locations.AnyAsync(l => l.Id == locationId);
    }

    public async Task<bool> ExistsByCityNameAsync(string cityName, long countryId)
    {
        return await _context.Locations
            .AnyAsync(l => l.CityName.ToLower() == cityName.ToLower() && l.CountryId == countryId);
    }

    public async Task<bool> ExistsByCityCodeAsync(string cityCode)
    {
        return await _context.Locations
            .AnyAsync(l => l.CityCode.ToUpper() == cityCode.ToUpper());
    }
}
