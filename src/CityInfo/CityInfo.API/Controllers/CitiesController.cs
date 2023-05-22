using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[ApiController]
public class CitiesController : ControllerBase
{
    [HttpGet(ApiEndpoints.Cities.GetAll)]
    public ActionResult<IEnumerable<CityDto>> GetCities()
    {
        return Ok(CitiesDataStore.Instance.Cities);
    }

    [HttpGet(ApiEndpoints.Cities.Get)]
    public ActionResult<CityDto> GetCity(int id)
    {
        CityDto? city = CitiesDataStore.Instance.Cities.FirstOrDefault(c => c.Id == id);
        return city is not null ? Ok(city) : NotFound();
    }
}
