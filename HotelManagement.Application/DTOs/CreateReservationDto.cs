public class CreateReservationDto
{
    public long RoomId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int NumberOfGuests { get; set; }
    public GuestDto Guest { get; set; }
    public EmergencyContactDto EmergencyContact { get; set; }
}
