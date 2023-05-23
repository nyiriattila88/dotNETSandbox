using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[Route("api/cities/{cityId}/pointsofinterests")]
[ApiController]
public class PointsOfInterestsController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterests(int cityId)
    {
        CityDto? city = CitiesDataStore.Instance.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city is null)
            return NotFound();

        return Ok(city.PointsOfInterests ?? Enumerable.Empty<PointOfInterestDto>());
    }

    [HttpGet("{pointofinterestid}", Name = nameof(GetPointOfInterest))]
    public ActionResult<PointOfInterestDto> GetPointOfInterest(
        int cityId, int pointOfInterestId)
    {
        CityDto? city = CitiesDataStore.Instance.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city is null)
            return NotFound();

        PointOfInterestDto? pointOfInterest = city.PointsOfInterests.FirstOrDefault(c => c.Id == pointOfInterestId);
        return pointOfInterest is not null ? Ok(pointOfInterest) : NotFound();
    }

    [HttpPost]
    public ActionResult<PointOfInterestDto> CreatePointOfInterest(int cityId, PointOfInterestCreateDto pointOfInterest)
    {
        CityDto? city = CitiesDataStore.Instance.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city is null)
            return NotFound();

        int maxPointOfInterestId = CitiesDataStore.Instance.Cities.SelectMany(c => c.PointsOfInterests).Max(p => p.Id);

        var finalPointOfInterest = new PointOfInterestDto()
        {
            Id = ++maxPointOfInterestId,
            Name = pointOfInterest.Name,
            Description = pointOfInterest.Description
        };

        city.PointsOfInterests.Add(finalPointOfInterest);

        return CreatedAtRoute(nameof(GetPointOfInterest),
             new
             {
                 cityId,
                 pointOfInterestId = finalPointOfInterest.Id
             },
             finalPointOfInterest);
    }
}
