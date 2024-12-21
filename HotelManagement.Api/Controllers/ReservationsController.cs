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

    [HttpGet("room/{roomId}")]
    public async Task<IActionResult> GetByRoomId(long roomId)
    {
        var reservations = await _reservationService.GetReservationsByRoomIdAsync(roomId);
        return Ok(reservations);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ReservationDto reservationDto)
    {
        await _reservationService.AddReservationAsync(reservationDto);
        return CreatedAtAction(nameof(GetByRoomId), new { roomId = reservationDto.RoomId }, reservationDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _reservationService.DeleteReservationAsync(id);
        return NoContent();
    }
}
