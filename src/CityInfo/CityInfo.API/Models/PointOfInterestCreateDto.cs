using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Models;

public class PointOfInterestCreateDto
{
    [Required(ErrorMessage = $"{nameof(Name)} is required.")]
    [MaxLength(50)]
    public required string Name { get; set; }

    [MaxLength(200)]
    public string? Description { get; set; }
}
