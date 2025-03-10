﻿using Microsoft.EntityFrameworkCore;

public class ReservationRepository : IReservationRepository
{
    private readonly ApplicationDbContext _context;

    public ReservationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Reservation>> GetReservationsByRoomIdAsync(long roomId)
    {
        return await _context.Reservations
            .Where(r => r.RoomId == roomId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetReservationsByDateRangeAsync(DateOnly startDate, DateOnly endDate)
    {
        return await _context.Reservations
            .Where(r => r.CheckInDate >= startDate && r.CheckOutDate <= endDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetAllAsync()
    {
        return await _context.Reservations.ToListAsync();
    }

    public async Task AddAsync(Reservation entity)
    {
        await _context.Reservations.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null) return;
        _context.Reservations.Remove(reservation);
        await _context.SaveChangesAsync();
    }

    public async Task<Reservation> GetByIdAsync(long id)
    {
        return await _context.Reservations
        .Include(r => r.Hotel)
            .ThenInclude(rg => rg.Location)
        .Include(r => r.Room)
        .Include(r => r.ReservationGuests)
            .ThenInclude(rg => rg.Guest)
        .FirstOrDefaultAsync(r => r.Id == id) ?? new Reservation();
    }

    public Task UpdateAsync(Reservation entity)
    {
        throw new NotImplementedException();
    }
}
