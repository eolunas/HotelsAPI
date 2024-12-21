using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomsController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpGet("{hotelId}")]
    public async Task<IActionResult> GetRoomsByHotelId(long hotelId)
    {
        var rooms = await _roomService.GetRoomsByHotelIdAsync(hotelId);
        return Ok(rooms);
    }

    [HttpPost]
    public async Task<IActionResult> Create(RoomDto roomDto)
    {
        await _roomService.AddRoomAsync(roomDto);
        return CreatedAtAction(nameof(GetRoomsByHotelId), new { hotelId = roomDto.HotelId }, roomDto);
    }

    [HttpPut]
    public async Task<IActionResult> Update(RoomDto roomDto)
    {
        await _roomService.UpdateRoomAsync(roomDto);
        return NoContent();
    }

    [HttpPatch("{roomId}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ToggleRoomStatus(long roomId, [FromBody] ToggleRoomStatusDto toggleStatusDto)
    {
        try
        {
            await _roomService.ToggleRoomStatusAsync(roomId, toggleStatusDto.isAvailable);
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _roomService.DeleteRoomAsync(id);
        return NoContent();
    }
}
