using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Hotels API - Endpoints for managing hotel.")]
public class HotelsController : ControllerBase
{
    private readonly IHotelService _hotelService;

    public HotelsController(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Retrieve all hotels with optional filters",
        Description = "Returns a list of hotels. You can filter by status, location, and the user who created them."
    )]
    public async Task<IActionResult> GetAll(
        [FromQuery, SwaggerParameter("Filter hotels by enabled status (true = only enabled, false = only disabled)", Required = false)]
    bool? isEnabled,

        [FromQuery, SwaggerParameter("Filter hotels by a specific location ID", Required = false)]
    long? locationId,

        [FromQuery, SwaggerParameter("Filter hotels by the user who created them", Required = false)]
    long? createdByUserId
    )
    {
        var hotels = await _hotelService.GetFilteredHotelsAsync(isEnabled, locationId, createdByUserId);
        return Ok(hotels);
    }


    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Retrieve a hotel by its ID",
        Description = "Returns the details of a specific hotel based on the provided ID. If the hotel is not found, it returns a 404 Not Found response."
    )]
    [SwaggerResponse(200, "Hotel details retrieved successfully", typeof(HotelDto))]
    [SwaggerResponse(404, "Hotel not found")]
    public async Task<IActionResult> GetById(
    [SwaggerParameter("The unique identifier of the hotel", Required = true)]
    long id)
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
    [SwaggerOperation(
        Summary = "Create a new hotel",
        Description = "Allows an admin user to create a new hotel. Requires authentication and an 'Admin' role."
    )]
    [SwaggerResponse(201, "Hotel created successfully")]
    [SwaggerResponse(400, "Invalid request data")]
    [SwaggerResponse(401, "Unauthorized - User ID not found in token")]
    [SwaggerResponse(403, "Forbidden - User does not have permission to perform this action")]
    [SwaggerResponse(404, "Not Found - Related resource not found")]
    [SwaggerResponse(409, "Conflict - Validation error")]
    [SwaggerResponse(500, "Internal Server Error - Unexpected error occurred")]
    public async Task<IActionResult> Create(
    [FromBody, SwaggerRequestBody("Hotel data required to create a new hotel", Required = true)]
    CreateHotelDto createHotelDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "User ID not found in token." });

            int userId = int.Parse(userIdClaim);

            await _hotelService.AddHotelAsync(createHotelDto, userId);
            return CreatedAtAction(nameof(Create), new { message = "Hotel created successfully." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ValidationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return StatusCode(403, new { message = "You do not have permission to perform this action." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
        }
    }

    [HttpPut("update")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(
        Summary = "Update an existing hotel",
        Description = "Allows an admin user to update hotel details, including name, location, and status."
    )]
    [SwaggerResponse(200, "Hotel updated successfully")]
    [SwaggerResponse(400, "Invalid request data")]
    [SwaggerResponse(404, "Not Found - Hotel not found")]
    [SwaggerResponse(500, "Internal Server Error - Unexpected error occurred")]
    public async Task<IActionResult> UpdateHotel(
    [FromBody, SwaggerRequestBody("Hotel data required to update an existing hotel", Required = true)]
    UpdateHotelDto updateHotelDto)
    {
        try
        {
            await _hotelService.UpdateHotelAsync(updateHotelDto);
            return Ok(new { message = "Hotel updated successfully." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(
        Summary = "Toggle the status of a hotel",
        Description = "Allows an admin user to enable or disable a hotel for reservations."
    )]
    [SwaggerResponse(200, "Hotel status updated successfully")]
    [SwaggerResponse(400, "Invalid request data")]
    [SwaggerResponse(404, "Hotel not found")]
    [SwaggerResponse(500, "Internal Server Error - Unexpected error occurred")]
    public async Task<IActionResult> ToggleHotelStatus(
    [SwaggerParameter("The unique identifier of the hotel", Required = true)]
    long id,

    [FromBody, SwaggerRequestBody("Indicates whether the hotel should be enabled or disabled", Required = true)]
    ToggleHotelStatusDto toggleStatusDto)
    {
        try
        {
            await _hotelService.ToggleHotelStatusAsync(id, toggleStatusDto.IsEnabled);
            return Ok(new
            {
                message = $"Hotel {(toggleStatusDto.IsEnabled ? "enabled" : "disabled")} successfully.",
                hotelId = id,
                newStatus = toggleStatusDto.IsEnabled
            });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Hotel with ID {id} not found." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "An unexpected error occurred.",
                details = ex.Message
            });
        }
    }

    [HttpDelete("delete/{id}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(
        Summary = "Delete a hotel",
        Description = "Allows an admin user to delete a hotel by its ID."
    )]
    [SwaggerResponse(200, "Hotel deleted successfully")]
    [SwaggerResponse(400, "Invalid request data")]
    [SwaggerResponse(404, "Hotel not found")]
    [SwaggerResponse(500, "Internal Server Error - Unexpected error occurred")]
    public async Task<IActionResult> Delete(
    [SwaggerParameter("The unique identifier of the hotel", Required = true)]
    long id)
    {
        try
        {
            await _hotelService.DeleteHotelAsync(id);
            return Ok(new
            {
                message = "Hotel deleted successfully.",
                hotelId = id
            });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Hotel with ID {id} not found." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "An unexpected error occurred while deleting the hotel.",
                details = ex.Message
            });
        }
    }

    [HttpPost("assign-rooms")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(
        Summary = "Assign rooms to a hotel",
        Description = "Allows an admin user to assign multiple rooms to a hotel by their ID properties."
    )]
    [SwaggerResponse(200, "Rooms successfully assigned to the hotel")]
    [SwaggerResponse(400, "Invalid request data")]
    [SwaggerResponse(404, "Hotel or rooms not found")]
    [SwaggerResponse(500, "Internal Server Error - Unexpected error occurred")]
    public async Task<IActionResult> AssignRoomsToHotel(
    [FromBody, SwaggerRequestBody("Hotel ID and list of room IDs to assign", Required = true)]
    AssignRoomsToHotelDto assignRoomsDto)
    {
        try
        {
            await _hotelService.AssignRoomsToHotelAsync(assignRoomsDto);
            return Ok(new
            {
                message = "Rooms successfully assigned to the hotel.",
                hotelId = assignRoomsDto.HotelId,
                assignedRooms = assignRoomsDto.RoomIds
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "An unexpected error occurred while assigning rooms.",
                details = ex.Message
            });
        }
    }

    [HttpPost("search")]
    [SwaggerOperation(
        Summary = "Search hotels by criteria",
        Description = "Search for hotels that match the given criteria, such as location, amenities, or availability."
    )]
    [SwaggerResponse(200, "Hotels found matching the search criteria", typeof(IEnumerable<HotelDto>))]
    [SwaggerResponse(400, "Invalid search criteria or validation error")]
    [SwaggerResponse(404, "No hotels found matching the criteria")]
    [SwaggerResponse(500, "Internal Server Error - Unexpected error occurred")]
    public async Task<IActionResult> SearchHotels(
    [FromBody, SwaggerRequestBody("Search criteria for finding hotels", Required = true)]
    HotelSearchCriteriaDto criteria)
    {
        if (criteria == null)
            return BadRequest(new { message = "Invalid search criteria. Please provide valid search parameters." });

        try
        {
            var results = await _hotelService.SearchHotelsAsync(criteria);

            if (!results.Any())
                return NotFound(new { message = "No hotels found matching your criteria." });

            return Ok(new
            {
                message = "Hotels found matching your criteria.",
                totalResults = results.Count(),
                hotels = results
            });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "An unexpected error occurred while searching for hotels.",
                details = ex.Message
            });
        }
    }

}
