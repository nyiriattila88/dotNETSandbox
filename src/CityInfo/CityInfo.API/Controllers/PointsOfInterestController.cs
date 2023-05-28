using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[Route("api/cities/{cityId}/pointsOfInterest")]
[ApiController]
public class PointsOfInterestController : ControllerBase
{
    private readonly CitiesDataStore _citiesDataStore;
    private readonly ILogger<PointsOfInterestController> _logger;
    private readonly LocalMailService _localMailService;

    public PointsOfInterestController(
        CitiesDataStore citiesDataStore,
        ILogger<PointsOfInterestController> logger,
        LocalMailService localMailService)
    {
        _citiesDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _localMailService = localMailService ?? throw new ArgumentNullException(nameof(localMailService));
    }

    [HttpGet]
    public ActionResult<IEnumerable<PointOfInterestDto>> GetAll(int cityId)
    {
        CityDto? city = FindCityByIdOrDefault(cityId);
        if (city is null)
            return NotFound();

        return Ok(city.PointsOfInterest ?? Enumerable.Empty<PointOfInterestDto>());
    }

    [HttpGet($"{{{nameof(pointOfInterestId)}}}", Name = nameof(GetById))]
    public ActionResult<PointOfInterestDto> GetById(int cityId, int pointOfInterestId)
    {
        CityDto? city = FindCityByIdOrDefault(cityId);
        if (city is null)
            return NotFound();

        PointOfInterestDto? pointOfInterest = FindPointOfInterestByIdOrDefault(city, pointOfInterestId);
        return pointOfInterest is not null ? Ok(pointOfInterest) : NotFound();
    }

    [HttpPost]
    public ActionResult<PointOfInterestDto> Create(int cityId, PointOfInterestCreateDto pointOfInterestCreateDto)
    {
        CityDto? city = FindCityByIdOrDefault(cityId);
        if (city is null)
            return NotFound();

        int maxPointOfInterestId = _citiesDataStore.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);

        var finalPointOfInterest = new PointOfInterestDto()
        {
            Id = ++maxPointOfInterestId,
            Name = pointOfInterestCreateDto.Name,
            Description = pointOfInterestCreateDto.Description
        };

        city.PointsOfInterest.Add(finalPointOfInterest);

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
        CityDto? city = FindCityByIdOrDefault(cityId);
        if (city is null)
            return NotFound();

        PointOfInterestDto? pointOfInterestFromStore = FindPointOfInterestByIdOrDefault(city, pointOfInterestId);
        if (pointOfInterestFromStore is null)
            return NotFound();

        pointOfInterestFromStore.Name = pointOfInterestUpdateDto.Name;
        pointOfInterestFromStore.Description = pointOfInterestUpdateDto.Description;

        return NoContent();
    }

    [HttpPatch($"{{{nameof(pointOfInterestId)}}}")]
    public ActionResult Patch(int cityId, int pointOfInterestId, PointOfInterestPatchDto pointOfInterestPatchDto)
    {
        CityDto? city = FindCityByIdOrDefault(cityId);
        if (city is null)
            return NotFound();

        PointOfInterestDto? pointOfInterestFromStore = FindPointOfInterestByIdOrDefault(city, pointOfInterestId);
        if (pointOfInterestFromStore is null)
            return NotFound();

        if (pointOfInterestPatchDto.Name is not null)
            pointOfInterestFromStore.Name = pointOfInterestPatchDto.Name.Value.NewValue;

        if (pointOfInterestPatchDto.Description is not null)
            pointOfInterestFromStore.Description = pointOfInterestPatchDto.Description.Value.NewValue;

        return NoContent();
    }

    [HttpDelete($"{{{nameof(pointOfInterestId)}}}")]
    public ActionResult Delete(int cityId, int pointOfInterestId)
    {
        CityDto? city = FindCityByIdOrDefault(cityId);
        if (city is null)
            return NotFound();

        PointOfInterestDto? pointOfInterestFromStore = FindPointOfInterestByIdOrDefault(city, pointOfInterestId);
        if (pointOfInterestFromStore is null)
            return NotFound();

        city.PointsOfInterest.Remove(pointOfInterestFromStore);
        _localMailService.Send("Point of interest deleted.", $"Point of interest {pointOfInterestFromStore.Name} with id {pointOfInterestFromStore.Id} was deleted.");

        return NoContent();
    }

    private CityDto? FindCityByIdOrDefault(int cityId)
    {
        CityDto? city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city is null)
            _logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");

        return city;
    }

    private PointOfInterestDto? FindPointOfInterestByIdOrDefault(CityDto city, int pointOfInterestId)
    {
        PointOfInterestDto? pointOfInterest = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointOfInterestId);
        if (pointOfInterest is null)
            _logger.LogInformation($"PointOfInterestDto with id {pointOfInterestId} wasn't found on City {city.Name}.");

        return pointOfInterest;
    }
}