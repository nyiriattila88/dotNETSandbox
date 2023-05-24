using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[Route("api/cities/{cityId}/pointsofinterests")]
[ApiController]
public class PointsOfInterestsController : ControllerBase
{
    private readonly CitiesDataStore _citiesDataStore;

    public PointsOfInterestsController(CitiesDataStore citiesDataStore)
    {
        _citiesDataStore = citiesDataStore;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PointOfInterestDto>> GetAll(int cityId)
    {
        CityDto? city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city is null)
            return NotFound();

        return Ok(city.PointsOfInterests ?? Enumerable.Empty<PointOfInterestDto>());
    }

    [HttpGet("{pointOfInterestId}", Name = nameof(GetById))]
    public ActionResult<PointOfInterestDto> GetById(int cityId, int pointOfInterestId)
    {
        CityDto? city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city is null)
            return NotFound();

        PointOfInterestDto? pointOfInterest = city.PointsOfInterests.FirstOrDefault(c => c.Id == pointOfInterestId);
        return pointOfInterest is not null ? Ok(pointOfInterest) : NotFound();
    }

    [HttpPost]
    public ActionResult<PointOfInterestDto> Create(int cityId, PointOfInterestCreateDto pointOfInterest)
    {
        CityDto? city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city is null)
            return NotFound();

        int maxPointOfInterestId = _citiesDataStore.Cities.SelectMany(c => c.PointsOfInterests).Max(p => p.Id);

        var finalPointOfInterest = new PointOfInterestDto()
        {
            Id = ++maxPointOfInterestId,
            Name = pointOfInterest.Name,
            Description = pointOfInterest.Description
        };

        city.PointsOfInterests.Add(finalPointOfInterest);

        return CreatedAtRoute(nameof(GetById),
             new
             {
                 cityId,
                 pointOfInterestId = finalPointOfInterest.Id
             },
             finalPointOfInterest);
    }

    [HttpPut("{pointOfInterestId}")]
    public ActionResult Update(int cityId, int pointOfInterestId, PointOfInterestUpdateDto pointOfInterest)
    {
        CityDto? city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city is null)
            return NotFound();

        PointOfInterestDto? pointOfInterestFromStore = city.PointsOfInterests.FirstOrDefault(c => c.Id == pointOfInterestId);
        if (pointOfInterestFromStore is null)
            return NotFound();

        pointOfInterestFromStore.Name = pointOfInterest.Name;
        pointOfInterestFromStore.Description = pointOfInterest.Description;

        return NoContent();
    }
}
