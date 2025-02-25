public class CreateReservationDto
{
    public long RoomId { get; set; }
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public int NumberOfGuests { get; set; }
    public List<GuestDto> Guests { get; set; }
    public EmergencyContactDto EmergencyContact { get; set; }
}
