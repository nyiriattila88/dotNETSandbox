namespace CityInfo.API.Models;

public record PointOfInterestCreateDto
{
    public required string Name { get; set; }

    public string? Description { get; set; }
}
