using CityInfo.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[Route("api/cities/{cityId}/pointsOfInterest")]
[ApiController]
public class PointsOfInterestController : ControllerBase
{
    private readonly CitiesDataStore _citiesDataStore;

    public PointsOfInterestController(CitiesDataStore citiesDataStore)
    {
        _citiesDataStore = citiesDataStore;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PointOfInterestDto>> GetAll(int cityId)
    {
        CityDto? city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city is null)
            return NotFound();

        return Ok(city.PointOfInterest ?? Enumerable.Empty<PointOfInterestDto>());
    }

    [HttpGet($"{{{nameof(pointOfInterestId)}}}", Name = nameof(GetById))]
    public ActionResult<PointOfInterestDto> GetById(int cityId, int pointOfInterestId)
    {
        CityDto? city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city is null)
            return NotFound();

        PointOfInterestDto? pointOfInterest = city.PointOfInterest.FirstOrDefault(c => c.Id == pointOfInterestId);
        return pointOfInterest is not null ? Ok(pointOfInterest) : NotFound();
    }

    [HttpPost]
    public ActionResult<PointOfInterestDto> Create(int cityId, PointOfInterestCreateDto pointOfInterest)
    {
        CityDto? city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city is null)
            return NotFound();

        int maxPointOfInterestId = _citiesDataStore.Cities.SelectMany(c => c.PointOfInterest).Max(p => p.Id);

        var finalPointOfInterest = new PointOfInterestDto()
        {
            Id = ++maxPointOfInterestId,
            Name = pointOfInterest.Name,
            Description = pointOfInterest.Description
        };

        city.PointOfInterest.Add(finalPointOfInterest);

        return CreatedAtRoute(nameof(GetById),
             new
             {
                 cityId,
                 pointOfInterestId = finalPointOfInterest.Id
             },
             finalPointOfInterest);
    }

    [HttpPut($"{{{nameof(pointOfInterestId)}}}")]
    public ActionResult Update(int cityId, int pointOfInterestId, PointOfInterestUpdateDto pointOfInterest)
    {
        CityDto? city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city is null)
            return NotFound();

        PointOfInterestDto? pointOfInterestFromStore = city.PointOfInterest.FirstOrDefault(c => c.Id == pointOfInterestId);
        if (pointOfInterestFromStore is null)
            return NotFound();

        pointOfInterestFromStore.Name = pointOfInterest.Name;
        pointOfInterestFromStore.Description = pointOfInterest.Description;

        return NoContent();
    }

    [HttpPatch($"{{{nameof(pointOfInterestId)}}}")]
    public ActionResult Patch(int cityId, int pointOfInterestId, JsonPatchDocument<PointOfInterestUpdateDto> patchDocument)
    {
        CityDto? city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city is null)
            return NotFound();

        PointOfInterestDto? pointOfInterestFromStore = city.PointOfInterest.FirstOrDefault(c => c.Id == pointOfInterestId);
        if (pointOfInterestFromStore is null)
            return NotFound();

        var pointOfInterestToPatch = new PointOfInterestUpdateDto()
        {
            Name = pointOfInterestFromStore.Name,
            Description = pointOfInterestFromStore.Description
        };

        patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!TryValidateModel(pointOfInterestToPatch))
            return BadRequest(ModelState);

        pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
        pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;

        return NoContent();
    }
}
