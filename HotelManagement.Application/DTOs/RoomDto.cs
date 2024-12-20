public class RoomDto
{
    public Guid Id { get; set; }
    public string RoomType { get; set; }
    public decimal BasePrice { get; set; }
    public decimal Taxes { get; set; }
    public string Location { get; set; }
    public bool IsAvailable { get; set; }
    public Guid HotelId { get; set; }
}
