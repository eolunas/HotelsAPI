using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

[ApiController]
[Route("api/countries")]
[SwaggerTag("Countries API - Endpoints for managing countries.")]
public class CountryController : ControllerBase
{
    private readonly ICountryService _countryService;

    public CountryController(ICountryService countryService)
    {
        _countryService = countryService;
    }

    /// <summary>
    /// Retrieve a list of all available countries.
    /// </summary>
    /// <returns>A list of countries.</returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all countries",
        Description = "Retrieves a list of all available countries in the system."
    )]
    [SwaggerResponse(200, "List of countries retrieved successfully", typeof(IEnumerable<CountryDto>))]
    [SwaggerResponse(500, "Internal Server Error - Unexpected error occurred")]
    public async Task<IActionResult> GetAllCountries()
    {
        var countries = await _countryService.GetAllCountriesAsync();
        return Ok(countries);
    }

    /// <summary>
    /// Create a new country.
    /// </summary>
    /// <param name="countryDto">Country creation data</param>
    /// <returns>The created country</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(
        Summary = "Create a new country",
        Description = "Creates a new country. Only accessible to users with the 'Admin' role."
    )]
    [SwaggerResponse(201, "Country created successfully", typeof(CountryDto))]
    [SwaggerResponse(400, "Invalid request data")]
    [SwaggerResponse(409, "Conflict - A country with the same name or code already exists")]
    [SwaggerResponse(500, "Internal Server Error - Unexpected error occurred")]
    public async Task<IActionResult> CreateCountry(
        [FromBody, SwaggerRequestBody("Country creation data", Required = true)]
        CreateCountryDto countryDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdCountry = await _countryService.CreateCountryAsync(countryDto);
            return CreatedAtAction(nameof(GetAllCountries), new { id = createdCountry.Id }, createdCountry);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
        }
    }
}
