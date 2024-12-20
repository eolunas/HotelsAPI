public class ReservationDto
{
    public Guid Id { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int NumberOfGuests { get; set; }
    public Guid RoomId { get; set; }
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public bool IsConfirmed { get; set; }
}
