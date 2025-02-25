public interface ICountryRepository : IRepository<Country>
{
    Task<bool> ExistsByNameAsync(string name);
    Task<bool> ExistsByCodeAsync(string countryCode);
}
