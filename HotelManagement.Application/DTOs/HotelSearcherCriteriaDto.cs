public class HotelSearchCriteriaDto
{
    public string City { get; set; } 
    public DateOnly CheckInDate { get; set; } 
    public DateOnly CheckOutDate { get; set; } 
    public int NumberOfGuests { get; set; } 
}
