namespace CityInfo.API.Models;

public record CityDto
{
    public required int Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public int NumberOfPointsOfInterest => PointsOfInterests.Count;

    public ICollection<PointOfInterestsDto> PointsOfInterests { get; set; } = new List<PointOfInterestsDto>();
}
