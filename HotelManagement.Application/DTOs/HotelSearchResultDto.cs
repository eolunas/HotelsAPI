﻿public class HotelSearchResultDto
{
    public long HotelId { get; set; } 
    public string HotelName { get; set; }
    public string Location { get; set; } 
    public IEnumerable<string> AvailableRoomTypes { get; set; } 
}
