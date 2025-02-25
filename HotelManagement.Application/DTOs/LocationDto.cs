public class LocationDto
{
    public long Id { get; set; }
    public string CityName { get; set; }
    public string CityCode { get; set; }
    public long CountryId { get; set; }
    public string CountryName { get; set; } 

    public LocationDto(Location location)
    {
        Id = location.Id;
        CityName = location.CityName;
        CityCode = location.CityCode;
        CountryId = location.CountryId;
        CountryName = location.Country?.Name ?? "Unknown";
    }
}
