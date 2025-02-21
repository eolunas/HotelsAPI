public class ReservationDetailDto
{
    public long Id { get; set; }
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public int NumberOfGuests { get; set; }
    public int Nights {  get; set; }
    public decimal TotalCost { get; set; }

    public GuestDto Guest { get; set; }
    public EmergencyContactDto EmergencyContact { get; set; }

    public HotelDetailDto Hotel { get; set; }
    public RoomDetailDto Room { get; set; }
}