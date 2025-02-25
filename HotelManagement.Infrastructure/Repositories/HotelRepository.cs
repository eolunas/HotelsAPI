using Microsoft.EntityFrameworkCore;

public class HotelRepository : IHotelRepository
{
    private readonly ApplicationDbContext _context;

    public HotelRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Hotel entity)
    {
        await _context.Hotels.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var hotel = await _context.Hotels.FindAsync(id);
        if (hotel == null) return;
        _context.Hotels.Remove(hotel);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Hotel>> GetAllAsync()
    {
        return await _context.Hotels
            .Include(h => h.Location)
            .Include(h => h.Rooms)
            .ToListAsync();
    }

    public async Task<Hotel> GetByIdAsync(long id)
    {
        return await _context.Hotels
            .Include(h => h.Location)
            .Include(h => h.Rooms)
            .FirstOrDefaultAsync(h => h.Id == id) ?? new Hotel();
    }

    public async Task UpdateAsync(Hotel entity)
    {
        _context.Hotels.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Hotel>> GetFilteredHotelsAsync(bool? isEnabled, long? locationId, long? createdByUserId)
    {
        var query = _context.Hotels
            .Include(h => h.Location)
            .Include(h => h.Rooms)
            .AsQueryable(); 

        if (isEnabled.HasValue)
        {
            query = query.Where(h => h.IsEnabled == isEnabled.Value);
        }

        if (locationId.HasValue)
        {
            query = query.Where(h => h.LocationId == locationId.Value);
        }

        if (createdByUserId.HasValue)
        {
            query = query.Where(h => h.CreatedByUserId == createdByUserId.Value);
        }

        return await query.ToListAsync(); 
    }

    public async Task<bool> ExistsInLocationAsync(string normalizedName, int locationId)
    {
        return await _context.Hotels
            .AnyAsync(h => h.Name.ToLower() == normalizedName && h.LocationId == locationId);
    }

}
