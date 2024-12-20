public class Guest
{
    public Guid Id { get; set; }
    public string FullName { get; set; } 
    public DateTime BirthDate { get; set; } 
    public string Gender { get; set; } 
    public string DocumentType { get; set; } 
    public string DocumentNumber { get; set; } 
    public string Email { get; set; } 
    public string Phone { get; set; }
    public Guid ReservationId { get; set; } 
    public Reservation Reservation { get; set; }
}
