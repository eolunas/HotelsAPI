using Microsoft.EntityFrameworkCore;

public class RoomRepository : IRoomRepository
{
    private readonly ApplicationDbContext _context;

    public RoomRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Room>> GetRoomsByHotelIdAsync(long hotelId)
    {
        return await _context.Rooms
            .Where(r => r.HotelId == hotelId)
            .ToListAsync();
    }

    public async Task AddAsync(Room entity)
    {
        await _context.Rooms.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null) return;
        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Room>> GetAllAsync()
    {
        return await _context.Rooms.ToListAsync();
    }

    public async Task<Room> GetByIdAsync(long id)
    {
        return await _context.Rooms
            .Include(r => r.Hotel)
            .FirstOrDefaultAsync(h => h.Id == id) ?? new Room();
    }

    public async Task UpdateAsync(Room entity)
    {
        _context.Rooms.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsyncRange(IEnumerable<Room> rooms)
    {
        _context.Rooms.UpdateRange(rooms);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Room>> GetFilteredRoomsAsync(bool? isAvailable, long? hotelId, long? maxNumberOfGuest)
    {
        var query = _context.Rooms
            .Include(h => h.Hotel)
            .AsQueryable();

        if (isAvailable.HasValue)
        {
            query = query.Where(h => h.IsAvailable == isAvailable.Value);
        }

        if (hotelId.HasValue)
        {
            query = query.Where(h => h.HotelId == hotelId.Value);
        }

        if (maxNumberOfGuest.HasValue)
        {
            query = query.Where(h => h.MaxNumberOfGuest == maxNumberOfGuest.Value);
        }

        return await query.ToListAsync();
    }

}
