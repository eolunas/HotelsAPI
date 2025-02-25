using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

[ApiController]
[Route("api/locations")]
[SwaggerTag("Locations API - Endpoints for managing locations within countries.")]
public class LocationController : ControllerBase
{
    private readonly ILocationService _locationService;

    public LocationController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    /// <summary>
    /// Retrieve all locations.
    /// </summary>
    /// <returns>A list of locations.</returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all locations",
        Description = "Retrieves a list of all available locations."
    )]
    [SwaggerResponse(200, "List of locations retrieved successfully", typeof(IEnumerable<LocationDto>))]
    [SwaggerResponse(500, "Internal Server Error - Unexpected error occurred")]
    public async Task<IActionResult> GetAllLocations()
    {
        var locations = await _locationService.GetAllLocationsAsync();
        return Ok(locations);
    }

    /// <summary>
    /// Retrieve locations by country.
    /// </summary>
    /// <param name="countryId">The ID of the country</param>
    /// <returns>A list of locations within the specified country.</returns>
    [HttpGet("country/{countryId}")]
    [SwaggerOperation(
        Summary = "Get locations by country",
        Description = "Retrieves locations associated with a specific country."
    )]
    [SwaggerResponse(200, "List of locations retrieved successfully", typeof(IEnumerable<LocationDto>))]
    [SwaggerResponse(404, "No locations found for this country")]
    [SwaggerResponse(500, "Internal Server Error - Unexpected error occurred")]
    public async Task<IActionResult> GetLocationsByCountry(
        [SwaggerParameter("The ID of the country for which locations should be retrieved")] 
        long countryId)
    {
        var locations = await _locationService.GetLocationsByCountryAsync(countryId);
        return locations.Any() ? Ok(locations) : NotFound(new { message = "No locations found for this country." });
    }

    /// <summary>
    /// Create a new location.
    /// </summary>
    /// <param name="locationDto">Location creation data</param>
    /// <returns>The created location.</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(
        Summary = "Create a new location",
        Description = "Creates a new location. Only accessible to users with the 'Admin' role."
    )]
    [SwaggerResponse(201, "Location created successfully", typeof(LocationDto))]
    [SwaggerResponse(400, "Invalid request data")]
    [SwaggerResponse(404, "Country not found")]
    [SwaggerResponse(409, "Conflict - A location with the same name or code already exists")]
    [SwaggerResponse(500, "Internal Server Error - Unexpected error occurred")]
    public async Task<IActionResult> CreateLocation(
        [FromBody, SwaggerRequestBody("Location creation data", Required = true)]
        CreateLocationDto locationDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdLocation = await _locationService.CreateLocationAsync(locationDto);
            return CreatedAtAction(nameof(GetAllLocations), new { id = createdLocation.Id }, createdLocation);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
        }
    }

}
