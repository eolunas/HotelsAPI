public class CountryService : ICountryService
{
    private readonly ICountryRepository _countryRepository;

    public CountryService(ICountryRepository countryRepository)
    {
        _countryRepository = countryRepository;
    }

    public async Task<IEnumerable<CountryDto>> GetAllCountriesAsync()
    {
        var Countries = await _countryRepository.GetAllAsync();
        return Countries.Select(c => new CountryDto(c));
    }

    public async Task<CountryDto> CreateCountryAsync(CreateCountryDto countryDto)
    {
        var existsByName = await _countryRepository.ExistsByNameAsync(countryDto.Name);
        if (existsByName)
            throw new InvalidOperationException($"A country named '{countryDto.Name}' already exists.");

        var existsByCode = await _countryRepository.ExistsByCodeAsync(countryDto.CountryCode.ToUpper());
        if (existsByCode)
            throw new InvalidOperationException($"A country with the code '{countryDto.CountryCode.ToUpper()}' already exists.");

        var country = new Country
        {
            Name = countryDto.Name.Trim(),
            CountryCode = countryDto.CountryCode.ToUpper()
        };

        await _countryRepository.AddAsync(country);
        return new CountryDto(country);
    }
}
