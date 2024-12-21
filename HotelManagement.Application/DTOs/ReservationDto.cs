public class ReservationDto
{
    public long Id { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int NumberOfGuests { get; set; }
    public long RoomId { get; set; }
    public bool IsConfirmed { get; set; }
}
