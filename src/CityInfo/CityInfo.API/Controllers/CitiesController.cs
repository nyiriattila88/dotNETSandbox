using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[ApiController]
[Route("api/cities")]
public class CitiesController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<CityDto>> GetAll()
    {
        return Ok(CitiesDataStore.Instance.Cities);
    }

    [HttpGet("{id}")]
    public ActionResult<CityDto> GetById(int id)
    {
        CityDto? city = CitiesDataStore.Instance.Cities.FirstOrDefault(c => c.Id == id);
        return city is not null ? Ok(city) : NotFound();
    }
}
