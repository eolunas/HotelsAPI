﻿using Microsoft.EntityFrameworkCore;

public class HotelRepository : IRepository<Hotel>
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

    public async Task DeleteAsync(Guid id)
    {
        var hotel = await _context.Hotels.FindAsync(id);
        if (hotel == null) return;
        _context.Hotels.Remove(hotel);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Hotel>> GetAllAsync()
    {
        return await _context.Hotels.ToListAsync();
    }

    public async Task<Hotel> GetByIdAsync(Guid id)
    {
        return await _context.Hotels.FindAsync(id);
    }

    public async Task UpdateAsync(Hotel entity)
    {
        _context.Hotels.Update(entity);
        await _context.SaveChangesAsync();
    }
}