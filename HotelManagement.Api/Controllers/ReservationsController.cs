using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Reservations API - Endpoints for managing hotel reservations.")]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationsController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    /// <summary>
    /// Retrieve all reservations.
    /// </summary>
    /// <returns>A list of reservations.</returns>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(
        Summary = "Get all reservations",
        Description = "Retrieves a list of all reservations in the system. Requires Admin role."
    )]
    [SwaggerResponse(200, "List of reservations retrieved successfully", typeof(IEnumerable<ReservationDto>))]
    [SwaggerResponse(403, "Forbidden - User does not have the necessary permissions")]
    [SwaggerResponse(500, "Internal Server Error - Unexpected error occurred")]
    public async Task<IActionResult> GetAllReservations()
    {
        var reservations = await _reservationService.GetAllReservationsAsync();
        return Ok(reservations);
    }

    /// <summary>
    /// Retrieve the details of a specific reservation.
    /// </summary>
    /// <param name="id">The unique ID of the reservation.</param>
    /// <returns>Returns the detailed information of the reservation.</returns>
    [HttpGet("{id}")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Get reservation details",
        Description = "Retrieves all details of a specific reservation, including check-in, check-out, guests, and total cost."
    )]
    [SwaggerResponse(200, "Reservation details retrieved successfully", typeof(ReservationDetailDto))]
    [SwaggerResponse(404, "Reservation not found")]
    [SwaggerResponse(500, "Internal Server Error - Unexpected error occurred")]
    public async Task<IActionResult> GetReservationDetail(
        [SwaggerParameter("The unique ID of the reservation to retrieve")] long id)
    {
        try
        {
            var reservationDetail = await _reservationService.GetReservationDetailAsync(id);
            return Ok(reservationDetail);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = $"Reservation with ID {id} not found." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
        }
    }

    /// <summary>
    /// Retrieve reservations for a specific room.
    /// </summary>
    /// <param name="roomId">The unique ID of the room.</param>
    /// <returns>Returns a list of reservations associated with the specified room.</returns>
    [HttpGet("room/{roomId}")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Get reservations by room ID",
        Description = "Retrieves all reservations associated with a specific room, including check-in and check-out dates."
    )]
    [SwaggerResponse(200, "Reservations retrieved successfully", typeof(IEnumerable<ReservationDto>))]
    [SwaggerResponse(404, "No reservations found for the given room ID")]
    [SwaggerResponse(500, "Internal Server Error - Unexpected error occurred")]
    public async Task<IActionResult> GetByRoomId(
        [SwaggerParameter("The unique ID of the room to retrieve reservations for")] long roomId)
    {
        var reservations = await _reservationService.GetReservationsByRoomIdAsync(roomId);
        return reservations.Any() ? Ok(reservations) : NotFound(new { message = $"No reservations found for room ID {roomId}." });
    }

    /// <summary>
    /// Create a new reservation.
    /// </summary>
    /// <param name="reservationDto">Reservation creation data</param>
    /// <returns>Returns a confirmation message if the reservation is successfully created.</returns>
    [HttpPost("create")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Create a new reservation",
        Description = "Creates a new reservation, assigning a guest to a room for a specified period. Requires authentication."
    )]
    [SwaggerResponse(201, "Reservation created successfully", typeof(object))]
    [SwaggerResponse(400, "Invalid reservation data or validation failed")]
    [SwaggerResponse(422, "Business rule violation - Room unavailable, invalid guests, etc.")]
    [SwaggerResponse(500, "Internal Server Error - Unexpected error occurred")]
    public async Task<IActionResult> CreateReservation(
        [FromBody, SwaggerRequestBody("Reservation creation data", Required = true)]
    CreateReservationDto reservationDto)
    {
        if (reservationDto == null)
        {
            return BadRequest(new { message = "Invalid reservation data." });
        }

        try
        {
            await _reservationService.CreateReservationAsync(reservationDto);
            return CreatedAtAction(nameof(GetByRoomId), new { roomId = reservationDto.RoomId },
                new { message = "Reservation created successfully." });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return UnprocessableEntity(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
        }
    }

    /// <summary>
    /// Cancel and delete an existing reservation.
    /// </summary>
    /// <param name="id">The unique ID of the reservation.</param>
    /// <returns>Returns a confirmation message if the reservation is successfully deleted.</returns>
    [HttpDelete("{id}")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Delete a reservation",
        Description = "Cancels and removes a reservation from the system. Requires authentication."
    )]
    [SwaggerResponse(200, "Reservation deleted successfully", typeof(object))]
    [SwaggerResponse(404, "Reservation not found")]
    [SwaggerResponse(500, "Internal Server Error - Unexpected error occurred")]
    public async Task<IActionResult> Delete(
        [SwaggerParameter("The unique ID of the reservation to delete")] long id)
    {
        try
        {
            await _reservationService.DeleteReservationAsync(id);
            return Ok(new { message = $"Reservation with ID {id} was successfully deleted." });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Reservation with ID {id} not found." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
        }
    }
}
