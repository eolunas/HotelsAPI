public class HotelDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public long LocationId { get; set; }
    public string Location { get; set; }
    public decimal LowPrice { get; set; }
    public bool IsEnabled { get; set; }
    public long CreatedByUserId { get; set; }

    public ICollection<RoomDto> Rooms { get; set; }

    public static HotelDto FromEntity(Hotel h)
    {
        return new HotelDto
        {
            Id = h.Id,
            Name = h.Name,
            LocationId = h.LocationId,
            Location = h.Location.CityName ?? "Unknown",
            LowPrice = h.Rooms != null && h.Rooms.Count != 0 ? h.Rooms.Min(r => r.BasePrice + r.Taxes) : 0,
            IsEnabled = h.IsEnabled,
            CreatedByUserId = h.CreatedByUserId,
            Rooms = h.Rooms?.Select(RoomDto.FromEntity).ToList() ?? []
        };
    }

}
