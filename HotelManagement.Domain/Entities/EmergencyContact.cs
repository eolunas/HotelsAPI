public class EmergencyContact
{
    public Guid Id { get; set; }
    public string FullName { get; set; } 
    public string Phone { get; set; }
    public Guid ReservationId { get; set; }
    public Reservation Reservation { get; set; }
}
