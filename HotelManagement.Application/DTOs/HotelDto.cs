public class HotelDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public decimal BasePrice { get; set; }
    public bool IsEnabled { get; set; }
    public long CreatedByUserId { get; set; }

    public ICollection<RoomDto> Rooms { get; set; }
}
