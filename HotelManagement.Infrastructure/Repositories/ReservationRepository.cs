using Microsoft.EntityFrameworkCore;

public class ReservationRepository : IReservationRepository
{
    private readonly ApplicationDbContext _context;

    public ReservationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Reservation>> GetReservationsByRoomIdAsync(Guid roomId)
    {
        return await _context.Reservations
            .Where(r => r.RoomId == roomId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetReservationsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Reservations
            .Where(r => r.CheckInDate >= startDate && r.CheckOutDate <= endDate)
            .ToListAsync();
    }

    public async Task AddAsync(Reservation entity)
    {
        await _context.Reservations.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null) return;
        _context.Reservations.Remove(reservation);
        await _context.SaveChangesAsync();
    }

    public Task<Reservation> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Reservation>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Reservation entity)
    {
        throw new NotImplementedException();
    }
}
