using Microsoft.EntityFrameworkCore;

public class GuestRepository : IGuestRepository
{
    private readonly ApplicationDbContext _context;

    public GuestRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guest> GetByIdAsync(long id)
    {
        return await _context.Guests.FindAsync(id);
    }

    public async Task<IEnumerable<Guest>> GetAllAsync()
    {
        return await _context.Guests.ToListAsync();
    }

    public async Task AddAsync(Guest guest)
    {
        await _context.Guests.AddAsync(guest);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Guest guest)
    {
        _context.Guests.Update(guest);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var guest = await _context.Guests.FindAsync(id);
        if (guest != null)
        {
            _context.Guests.Remove(guest);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await _context.Guests.AnyAsync(g => g.Id == id);
    }

    public async Task<Guest> AddOrUpdateAsync(Guest guest)
    {
        var existingGuest = await _context.Guests
            .FirstOrDefaultAsync(g => g.DocumentType == guest.DocumentType && g.DocumentNumber == guest.DocumentNumber);

        if (existingGuest != null)
        {
            // Actualizar campos si ya existe
            existingGuest.FullName = guest.FullName;
            existingGuest.Email = guest.Email;
            existingGuest.Phone = guest.Phone;

            _context.Guests.Update(existingGuest);
            await _context.SaveChangesAsync();
            return existingGuest;
        }

        // Agregar un nuevo huésped si no existe
        await _context.Guests.AddAsync(guest);
        await _context.SaveChangesAsync();
        return guest;
    }
}
