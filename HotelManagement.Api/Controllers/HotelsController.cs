using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class HotelsController : ControllerBase
{
    private readonly IHotelService _hotelService;

    public HotelsController(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var hotels = await _hotelService.GetAllHotelsAsync();
        return Ok(hotels);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var hotel = await _hotelService.GetHotelByIdAsync(id);
            return Ok(hotel);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(HotelDto hotelDto)
    {
        await _hotelService.AddHotelAsync(hotelDto);
        return CreatedAtAction(nameof(GetById), new { id = hotelDto.Id }, hotelDto);
    }

    [HttpPost]
    [Route("search")]
    public async Task<IActionResult> SearchHotels([FromBody] HotelSearchCriteriaDto criteria)
    {
        if (criteria == null) return BadRequest("Invalid search criteria.");

        var results = await _hotelService.SearchHotelsAsync(criteria);

        if (!results.Any()) return NotFound("No hotels found matching your criteria.");

        return Ok(results);
    }

}
