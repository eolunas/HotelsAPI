public class HotelSearchCriteriaDto
{
    public int LocationId { get; set; }
    public DateOnly CheckInDate { get; set; } 
    public DateOnly CheckOutDate { get; set; } 
    public int NumberOfGuests { get; set; } 
}
