using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
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

    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateHotelDto createHotelDto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        await _hotelService.AddHotelAsync(createHotelDto, userId);
        return Ok(new { message = "Hotel created successfully." });
    }

    [HttpPut("update")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateHotel(UpdateHotelDto updateHotelDto)
    {
        try
        {
            await _hotelService.UpdateHotelAsync(updateHotelDto);
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

    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(
        Summary = "Update the status of a hotel", 
        Description = "Enables or disables a hotel for reservations"
    )]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ToggleHotelStatus(long id, [FromBody] ToggleHotelStatusDto toggleStatusDto)
    {
        try
        {
            await _hotelService.ToggleHotelStatusAsync(id, toggleStatusDto.IsEnabled);
            return NoContent(); // 204 No Content
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Hotel with ID {id} not found."); // 404 Not Found
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occurred."); // 500 Internal Server Error
        }
    }


    [HttpDelete("delete/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(long id)
    {
        await _hotelService.DeleteHotelAsync(id);
        return NoContent();
    }

    [HttpPost("assign-rooms")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(
        Summary = "Assign rooms to hotel", 
        Description = "Assign n rooms to one hotel by their id properties"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

    [HttpPost("search")]
    [SwaggerOperation(
        Summary = "Search hotels by criteria",
        Description = "Search for hotels that match the given criteria such as location, amenities, or availability."
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<HotelDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SearchHotels([FromBody] HotelSearchCriteriaDto criteria)
    {
        if (criteria == null)
            return BadRequest("Invalid search criteria.");

        try
        {
            var results = await _hotelService.SearchHotelsAsync(criteria);

            if (!results.Any())
                return NotFound("No hotels found matching your criteria.");

            return Ok(results);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

}
