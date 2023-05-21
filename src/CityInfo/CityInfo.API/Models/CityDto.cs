namespace CityInfo.API.Models;

public record CityDto
{
    public required int Id { get; set; }
    public required string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
