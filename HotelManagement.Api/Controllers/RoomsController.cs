using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomsController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var rooms = await _roomService.GetAllRoomsAsync();
        return Ok(rooms);
    }

    [HttpGet("{hotelId}")]
    public async Task<IActionResult> GetRoomsByHotelId(long hotelId)
    {
        var rooms = await _roomService.GetRoomsByHotelIdAsync(hotelId);
        return Ok(rooms);
    }

    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateRoomDto createRoomDto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        await _roomService.AddRoomAsync(createRoomDto, userId);
        return Ok(new { message = "Room created successfully." });
    }

    [HttpPut("update")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(UpdateRoomDto updateRoomDto)
    {
        try
        {
            await _roomService.UpdateRoomAsync(updateRoomDto);
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

    [HttpPatch("{roomId}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ToggleRoomStatus(long roomId, [FromBody] ToggleRoomStatusDto toggleStatusDto)
    {
        try
        {
            await _roomService.ToggleRoomStatusAsync(roomId, toggleStatusDto.isAvailable);
            return NoContent(); // 204 No Content
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Room with ID {roomId} not found."); // 404 Not Found
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
        await _roomService.DeleteRoomAsync(id);
        return NoContent();
    }
}
