public interface IGuestRepository : IRepository<Guest>
{
    Task<Guest> AddOrUpdateAsync(Guest guest);
}
