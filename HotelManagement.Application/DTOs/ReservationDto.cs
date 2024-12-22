public class ReservationDto
{
    public long Id { get; set; }
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public int NumberOfGuests { get; set; }
    public long RoomId { get; set; }
    public bool IsConfirmed { get; set; }
}
