namespace CityInfo.API.Models;

public record CityDto
{
    public required int Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public int NumberOfPointsOfInterest => PointsOfInterests?.Count ?? 0;

    public ICollection<PointOfInterestDto> PointsOfInterests { get; set; } = new List<PointOfInterestDto>();
}