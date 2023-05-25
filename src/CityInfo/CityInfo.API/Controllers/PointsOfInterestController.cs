using CityInfo.API.Models;
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
    public ActionResult<PointOfInterestDto> Create(int cityId, PointOfInterestCreateDto pointOfInterestCreateDto)
    {
        CityDto? city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city is null)
            return NotFound();

        int maxPointOfInterestId = _citiesDataStore.Cities.SelectMany(c => c.PointOfInterest).Max(p => p.Id);

        var finalPointOfInterest = new PointOfInterestDto()
        {
            Id = ++maxPointOfInterestId,
            Name = pointOfInterestCreateDto.Name,
            Description = pointOfInterestCreateDto.Description
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
    public ActionResult Update(int cityId, int pointOfInterestId, PointOfInterestUpdateDto pointOfInterestUpdateDto)
    {
        CityDto? city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city is null)
            return NotFound();

        PointOfInterestDto? pointOfInterestFromStore = city.PointOfInterest.FirstOrDefault(c => c.Id == pointOfInterestId);
        if (pointOfInterestFromStore is null)
            return NotFound();

        pointOfInterestFromStore.Name = pointOfInterestUpdateDto.Name;
        pointOfInterestFromStore.Description = pointOfInterestUpdateDto.Description;

        return NoContent();
    }

    [HttpPatch($"{{{nameof(pointOfInterestId)}}}")]
    public ActionResult Patch(int cityId, int pointOfInterestId, PointOfInterestPatchDto pointOfInterestPatchDto)
    {
        CityDto? city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city is null)
            return NotFound();

        PointOfInterestDto? pointOfInterestFromStore = city.PointOfInterest.FirstOrDefault(c => c.Id == pointOfInterestId);
        if (pointOfInterestFromStore is null)
            return NotFound();

        var pointOfInterestUpdateDto = new PointOfInterestUpdateDto()
        {
            Name = pointOfInterestPatchDto.Name is not null ? pointOfInterestPatchDto.Name.Value.NewValue : pointOfInterestFromStore.Name,
            Description = pointOfInterestPatchDto.Description is not null ? pointOfInterestPatchDto.Description.Value.NewValue : pointOfInterestFromStore.Description,
        };

        return Update(cityId, pointOfInterestId, pointOfInterestUpdateDto);
    }
}
