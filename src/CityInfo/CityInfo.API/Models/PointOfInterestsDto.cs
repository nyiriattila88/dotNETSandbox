namespace CityInfo.API.Models;

public record PointOfInterestsDto
{
    public required int Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }
}
