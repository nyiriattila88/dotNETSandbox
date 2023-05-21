using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[ApiController]
public class CitiesController : ControllerBase
{
    [HttpGet(ApiEndpoints.Cities.GetAll)]
    public JsonResult GetCities() => new(CitiesDataStore.Instance.Cities);

    [HttpGet(ApiEndpoints.Cities.Get)]
    public JsonResult GetCity(int id) => new(CitiesDataStore.Instance.Cities.FirstOrDefault(c => c.Id == id));
}
