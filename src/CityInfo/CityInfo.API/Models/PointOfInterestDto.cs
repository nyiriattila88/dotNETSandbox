namespace CityInfo.API.Models;

public record PointOfInterestDto
{
    public required int Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }
}
