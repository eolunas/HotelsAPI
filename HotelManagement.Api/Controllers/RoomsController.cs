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
    public async Task<IActionResult> GetRoomsByHotelId(Guid hotelId)
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _roomService.DeleteRoomAsync(id);
        return NoContent();
    }
}
