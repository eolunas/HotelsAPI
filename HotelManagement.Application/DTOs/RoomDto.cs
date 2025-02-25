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

    public static RoomDto FromEntity(Room r)
    {
        return new RoomDto
        {
            Id = r.Id,
            RoomType = r.RoomType,
            Taxes = r.Taxes,
            BasePrice = r.BasePrice,
            Location = r.Location,
            MaxNumberOfGuest = r.MaxNumberOfGuest,
            IsAvailable = r.IsAvailable,
            HotelId = r.HotelId
        };
    }

}
