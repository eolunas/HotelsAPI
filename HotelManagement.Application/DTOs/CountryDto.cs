public class CountryDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string CountryCode { get; set; }

    public CountryDto(Country country)
    {
        Id = country.Id;
        Name = country.Name;
        CountryCode = country.CountryCode;
    }
}
