using Microsoft.EntityFrameworkCore;

public class CountryRepository : ICountryRepository
{
    private readonly ApplicationDbContext _context;

    public CountryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Country> GetByIdAsync(long id)
    {
        return await _context.Countries.FindAsync(id);
    }

    public async Task<IEnumerable<Country>> GetAllAsync()
    {
        return await _context.Countries.ToListAsync();
    }

    public async Task AddAsync(Country entity)
    {
        await _context.Countries.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Country entity)
    {
        _context.Countries.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var country = await _context.Countries.FindAsync(id);
        if (country == null) return;
        _context.Countries.Remove(country);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _context.Countries.AnyAsync(c => c.Name.ToLower() == name.ToLower());
    }

    public async Task<bool> ExistsByCodeAsync(string countryCode)
    {
        return await _context.Countries.AnyAsync(c => c.CountryCode.ToUpper() == countryCode.ToUpper());
    }
}
