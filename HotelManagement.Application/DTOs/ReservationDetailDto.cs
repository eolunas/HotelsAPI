public class ReservationDetailDto
{
    public long Id { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int NumberOfGuests { get; set; }
    public decimal TotalCost { get; set; }

    public GuestDto Guest { get; set; }
    public EmergencyContactDto EmergencyContact { get; set; }
    public RoomDto Room { get; set; }
}