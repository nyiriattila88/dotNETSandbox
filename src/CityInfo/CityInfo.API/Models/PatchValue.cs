namespace CityInfo.API.Models;

public readonly struct PatchValue<T>
{
    public T NewValue { get; init; }
}