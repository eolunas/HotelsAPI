public interface IEmergencyContactRepository
{
    Task AddAsync(EmergencyContact contact);
    Task<EmergencyContact> GetByReservationIdAsync(long ReservationId);
}
