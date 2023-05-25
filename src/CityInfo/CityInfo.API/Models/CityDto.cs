using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Models;

public record CityDto
{
    [Required(ErrorMessage = $"{nameof(Id)} is required.")]
    public required int Id { get; set; }

    [Required(ErrorMessage = $"{nameof(Name)} is required.")]
    public required string Name { get; set; }

    public string? Description { get; set; }

    public int NumberOfPointsOfInterest => PointOfInterest?.Count ?? 0;

    public ICollection<PointOfInterestDto> PointOfInterest { get; set; } = new List<PointOfInterestDto>();
}