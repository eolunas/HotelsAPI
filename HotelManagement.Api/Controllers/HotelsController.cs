using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
    public async Task<IActionResult> GetById(long id)
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

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateHotelDto createHotelDto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        await _hotelService.AddHotelAsync(createHotelDto, userId);
        return Ok(new { message = "Hotel created successfully." });
    }

    [HttpPut("{hotelId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateHotel(long hotelId, [FromBody] UpdateHotelDto updateHotelDto)
    {
        try
        {
            await _hotelService.UpdateHotelAsync(hotelId, updateHotelDto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message); 
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occurred."); 
        }
    }

    [HttpPatch("{hotelId}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ToggleHotelStatus(long hotelId, [FromBody] ToggleHotelStatusDto toggleStatusDto)
    {
        try
        {
            await _hotelService.ToggleHotelStatusAsync(hotelId, toggleStatusDto.IsEnabled);
            return NoContent(); // 204 No Content
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message); // 404 Not Found
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occurred."); // 500 Internal Server Error
        }
    }


    [HttpPost("assign-rooms")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignRoomsToHotel([FromBody] AssignRoomsToHotelDto assignRoomsDto)
    {
        try
        {
            await _hotelService.AssignRoomsToHotelAsync(assignRoomsDto);
            return Ok("Rooms successfully assigned to the hotel.");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occurred.");
        }
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
