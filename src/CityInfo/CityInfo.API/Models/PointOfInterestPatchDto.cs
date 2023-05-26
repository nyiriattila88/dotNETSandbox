namespace CityInfo.API.Models;

public class PointOfInterestPatchDto
{
    public PatchValue<string>? Name { get; set; }

    public PatchValue<string?>? Description { get; set; }
}
