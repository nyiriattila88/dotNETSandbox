namespace CityInfo.API.Models;

public record PointOfInterestPatchDto
{
    public PatchValue<string>? Name { get; set; }

    public PatchValue<string?>? Description { get; set; }
}
