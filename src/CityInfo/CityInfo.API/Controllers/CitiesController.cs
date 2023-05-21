using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[ApiController]
public class CitiesController : ControllerBase
{
    [HttpGet(ApiEndpoints.Cities.GetAll)]
    public JsonResult GetCities() => new JsonResult(CitiesDataStore.Instance.Cities);
}
