using Microsoft.EntityFrameworkCore;

public class EmergencyContactRepository : IEmergencyContactRepository
{
    private readonly ApplicationDbContext _context;

    public EmergencyContactRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EmergencyContact> GetByReservationIdAsync(long reservationId)
    {
        return await _context.EmergencyContacts
            .FirstOrDefaultAsync(ec => ec.ReservationId == reservationId) ?? new EmergencyContact();
    }

    public async Task AddAsync(EmergencyContact contact)
    {
        await _context.EmergencyContacts.AddAsync(contact);
        await _context.SaveChangesAsync();
    }
}
