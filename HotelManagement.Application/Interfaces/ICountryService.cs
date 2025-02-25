public interface ICountryService
{
    Task<IEnumerable<CountryDto>> GetAllCountriesAsync();
    Task<CountryDto> CreateCountryAsync(CreateCountryDto countryDto);
}
