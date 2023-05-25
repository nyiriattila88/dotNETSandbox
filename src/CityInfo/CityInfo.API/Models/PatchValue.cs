namespace CityInfo.API.Models;

public readonly record struct PatchValue<T>(T NewValue);
