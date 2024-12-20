using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class HotelSearchController : ControllerBase
{
    private readonly IHotelSearchService _hotelSearchService;

    public HotelSearchController(IHotelSearchService hotelSearchService)
    {
        _hotelSearchService = hotelSearchService;
    }

    [HttpPost]
    [Route("search")]
    public async Task<IActionResult> SearchHotels([FromBody] HotelSearchCriteriaDto criteria)
    {
        if (criteria == null) return BadRequest("Invalid search criteria.");

        var results = await _hotelSearchService.SearchHotelsAsync(criteria);

        if (!results.Any()) return NotFound("No hotels found matching your criteria.");

        return Ok(results);
    }
}
