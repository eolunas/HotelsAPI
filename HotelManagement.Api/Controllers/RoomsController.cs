using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Hotels API - Endpoints for managing rooms.")]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomsController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    /// <summary>
    /// Retrieve a list of rooms with optional filters.
    /// </summary>
    /// <param name="isAvailable">Filter by availability status (true for available, false for occupied).</param>
    /// <param name="hotelId">Filter by hotel ID to get rooms for a specific hotel.</param>
    /// <param name="maxNumberOfGuest">Filter by maximum guest capacity.</param>
    /// <returns>Returns a filtered list of rooms based on the given criteria.</returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all rooms with filters",
        Description = "Retrieves a list of rooms. Supports optional filters for availability, hotel association, and guest capacity."
    )]
    [SwaggerResponse(200, "Rooms retrieved successfully", typeof(IEnumerable<RoomDto>))]
    [SwaggerResponse(400, "Invalid query parameters")]
    [SwaggerResponse(500, "Internal Server Error - Unexpected error occurred")]
    public async Task<IActionResult> GetAll(
        [FromQuery, SwaggerParameter("Filter by availability status (true for available, false for occupied)")]
    bool? isAvailable,

        [FromQuery, SwaggerParameter("Filter by hotel ID to retrieve rooms from a specific hotel")]
    long? hotelId,

        [FromQuery, SwaggerParameter("Filter by maximum number of guests allowed in the room")]
    long? maxNumberOfGuest)
    {
        try
        {
            var rooms = await _roomService.GetFilteredRoomsAsync(isAvailable, hotelId, maxNumberOfGuest);

            if (!rooms.Any())
                return NotFound(new { message = "No rooms match the specified criteria." });

            return Ok(rooms);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
        }
    }

    /// <summary>
    /// Retrieve all rooms for a specific hotel.
    /// </summary>
    /// <param name="hotelId">The unique ID of the hotel.</param>
    /// <returns>Returns a list of rooms available in the specified hotel.</returns>
    [HttpGet("{hotelId}")]
    [SwaggerOperation(
        Summary = "Get rooms by hotel ID",
        Description = "Retrieves all rooms associated with a specific hotel, including room type, price, and availability."
    )]
    [SwaggerResponse(200, "Rooms retrieved successfully", typeof(IEnumerable<RoomDto>))]
    [SwaggerResponse(404, "No rooms found for the given hotel ID")]
    [SwaggerResponse(500, "Internal Server Error - Unexpected error occurred")]
    public async Task<IActionResult> GetRoomsByHotelId(
        [SwaggerParameter("The unique ID of the hotel to retrieve rooms for")] long hotelId)
    {
        try
        {
            var rooms = await _roomService.GetRoomsByHotelIdAsync(hotelId);

            if (!rooms.Any())
                return NotFound(new { message = $"No rooms found for hotel ID {hotelId}." });

            return Ok(rooms);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
        }
    }

    /// <summary>
    /// Create a new room in a hotel.
    /// </summary>
    /// <param name="createRoomDto">The room details for creation.</param>
    /// <returns>Returns a confirmation message if the room is successfully created.</returns>
    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(
        Summary = "Create a new room",
        Description = "Creates a new room in a specified hotel. Only accessible by admins."
    )]
    [SwaggerResponse(201, "Room created successfully", typeof(object))]
    [SwaggerResponse(400, "Invalid request data")]
    [SwaggerResponse(403, "Forbidden - User does not have the necessary permissions")]
    [SwaggerResponse(500, "Internal Server Error - Unexpected error occurred")]
    public async Task<IActionResult> Create(
        [FromBody, SwaggerRequestBody("Room creation data", Required = true)]
    CreateRoomDto createRoomDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "User ID not found in token." });

            int userId = int.Parse(userIdClaim);

            await _roomService.AddRoomAsync(createRoomDto, userId);
            return CreatedAtAction(nameof(Create), new { message = "Room created successfully." });
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

    /// <summary>
    /// Update an existing room's details.
    /// </summary>
    /// <param name="updateRoomDto">The updated room details.</param>
    /// <returns>Returns a confirmation message if the update is successful.</returns>
    [HttpPut("update")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(
        Summary = "Update an existing room",
        Description = "Modifies details of an existing room, such as price, availability, or room type. Only accessible by admins."
    )]
    [SwaggerResponse(200, "Room updated successfully", typeof(object))]
    [SwaggerResponse(400, "Invalid request data")]
    [SwaggerResponse(404, "Room not found")]
    [SwaggerResponse(500, "Internal Server Error - Unexpected error occurred")]
    public async Task<IActionResult> Update(
        [FromBody, SwaggerRequestBody("Updated room details", Required = true)]
    UpdateRoomDto updateRoomDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _roomService.UpdateRoomAsync(updateRoomDto);
            return Ok(new { message = $"Room with ID {updateRoomDto.Id} updated successfully." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
        }
    }

    /// <summary>
    /// Enable or disable a room's availability.
    /// </summary>
    /// <param name="roomId">The unique ID of the room.</param>
    /// <param name="toggleStatusDto">Indicates whether the room should be available or not.</param>
    /// <returns>Returns a confirmation message if the status is successfully updated.</returns>
    [HttpPatch("{roomId}/status")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(
        Summary = "Toggle room availability status",
        Description = "Updates the availability status of a specific room. Only accessible by admins."
    )]
    [SwaggerResponse(200, "Room status updated successfully", typeof(object))]
    [SwaggerResponse(400, "Invalid request data")]
    [SwaggerResponse(404, "Room not found")]
    [SwaggerResponse(500, "Internal Server Error - Unexpected error occurred")]
    public async Task<IActionResult> ToggleRoomStatus(
        [SwaggerParameter("The unique ID of the room to update availability for")] long roomId,
        [FromBody, SwaggerRequestBody("Room status update data", Required = true)]
    ToggleRoomStatusDto toggleStatusDto)
    {
        try
        {
            if (toggleStatusDto == null)
                return BadRequest(new { message = "Invalid request data." });

            await _roomService.ToggleRoomStatusAsync(roomId, toggleStatusDto.isAvailable);
            return Ok(new { message = $"Room with ID {roomId} availability updated to {toggleStatusDto.isAvailable}." });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Room with ID {roomId} not found." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete an existing room.
    /// </summary>
    /// <param name="id">The unique ID of the room.</param>
    /// <returns>Returns a confirmation message if the room is successfully deleted.</returns>
    [HttpDelete("delete/{id}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(
        Summary = "Delete a room",
        Description = "Deletes a room permanently from the system. Only accessible by admins."
    )]
    [SwaggerResponse(200, "Room deleted successfully", typeof(object))]
    [SwaggerResponse(404, "Room not found")]
    [SwaggerResponse(500, "Internal Server Error - Unexpected error occurred")]
    public async Task<IActionResult> Delete(
        [SwaggerParameter("The unique ID of the room to delete")] long id)
    {
        try
        {
            await _roomService.DeleteRoomAsync(id);
            return Ok(new { message = $"Room with ID {id} was successfully deleted." });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Room with ID {id} not found." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
        }
    }
}
