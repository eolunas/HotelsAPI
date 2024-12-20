public class Hotel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public decimal BasePrice { get; set; }
    public bool IsEnabled { get; set; }
    public ICollection<Room> Rooms { get; set; }
}
