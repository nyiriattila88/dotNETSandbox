using CityInfo.API.Models;

namespace CityInfo.API;

/// <summary>
/// In memory data before EF is added
/// </summary>
public class CitiesDataStore
{
    private static readonly Lazy<CitiesDataStore> instance = new(() => new CitiesDataStore());
    public static CitiesDataStore Instance { get; } = instance.Value;

    private CitiesDataStore()
    {
        Cities = new List<CityDto>
        {
            new CityDto()
            {
                Id = 1,
                Name = "New York City",
                Description = "A city from the USA.",
            },

            new CityDto()
            {
                Id = 2,
                Name = "London",
                Description = "A city from the UK.",
            },

            new CityDto()
            {
                Id = 3,
                Name = "Paris",
                Description = "A city from France.",
            },
        };
    }

    public IEnumerable<CityDto> Cities { get; }
}
