using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[Route("api/cities/{cityId}/pointsofinterests")]
[ApiController]
public class PointsOfInterestsController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<PointOfInterestsDto>> GetPointsOfInterests(int cityId)
    {
        CityDto? city = CitiesDataStore.Instance.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city is null)
            return NotFound();

        return Ok(city.PointsOfInterests ?? Enumerable.Empty<PointOfInterestsDto>());
    }

    [HttpGet("{pointOfInterestId}")]
    public ActionResult<PointOfInterestsDto> GetPointsOfInterests(int cityId, int pointOfInterestId)
    {
        CityDto? city = CitiesDataStore.Instance.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city is null)
            return NotFound();

        PointOfInterestsDto? pointOfInterest = city.PointsOfInterests.FirstOrDefault(p => p.Id == pointOfInterestId);
        return pointOfInterest is not null ? Ok(pointOfInterest) : NotFound();
    }
}
