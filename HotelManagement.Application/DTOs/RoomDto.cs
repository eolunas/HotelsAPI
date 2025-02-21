public class RoomDto
{
    public long Id { get; set; }
    public string RoomType { get; set; }
    public decimal BasePrice { get; set; }
    public decimal Taxes { get; set; }
    public int MaxNumberOfGuest { get; set; }
    public string Location { get; set; }
    public bool IsAvailable { get; set; }
    public long? HotelId { get; set; }
}
