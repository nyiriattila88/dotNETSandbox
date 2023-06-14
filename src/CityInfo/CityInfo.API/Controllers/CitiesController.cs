using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[ApiController]
[Route("api/cities")]
public class CitiesController : ControllerBase
{
    private readonly CitiesDataStore _citiesDataStore;

    public CitiesController(CitiesDataStore citiesDataStore)
    {
        _citiesDataStore = citiesDataStore;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CityDto>>> GetAll() => Ok(_citiesDataStore.Cities);

    [HttpGet($"{{{nameof(id)}}}")]
    public async Task<ActionResult<CityDto>> GetById(int id)
    {
        CityDto? city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == id);
        return city is not null ? Ok(city) : NotFound();
    }
}
