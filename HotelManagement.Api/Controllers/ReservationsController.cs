using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationsController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllReservations()
    {
        var reservations = await _reservationService.GetAllReservationsAsync();
        return Ok(reservations);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetReservationDetail(long id)
    {
        try
        {
            var reservationDetail = await _reservationService.GetReservationDetailAsync(id);
            return Ok(reservationDetail);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("room/{roomId}")]
    public async Task<IActionResult> GetByRoomId(long roomId)
    {
        var reservations = await _reservationService.GetReservationsByRoomIdAsync(roomId);
        return Ok(reservations);
    }
    
    [HttpPost("create")]
    public async Task<IActionResult> CreateReservation(CreateReservationDto reservationDto)
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _reservationService.DeleteReservationAsync(id);
        return NoContent();
    }
}
