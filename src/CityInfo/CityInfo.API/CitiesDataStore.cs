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
                PointsOfInterests = new List<PointOfInterestsDto>
                {
                    new PointOfInterestsDto
                    {
                        Id = 1,
                        Name = "Central Park",
                        Description = "The most visited park in the US."
                    },
                    new PointOfInterestsDto
                    {
                        Id = 2,
                        Name = "Empire State Building",
                        Description = "A skyscraper located in Midtown Manhattan."
                    },
                },
            },
            new CityDto()
            {
                Id = 2,
                Name = "London",
                Description = "A city from the UK.",
                PointsOfInterests = new List<PointOfInterestsDto>
                {
                    new PointOfInterestsDto
                    {
                        Id = 1,
                        Name = "Big Ben",
                        Description = "Iconic building of London."
                    },
                }
            },
            new CityDto()
            {
                Id = 3,
                Name = "Paris",
                Description = "A city from France.",
                PointsOfInterests = new List<PointOfInterestsDto>
                {
                    new PointOfInterestsDto
                    {
                        Id = 1,
                        Name = "Eiffel Tower",
                        Description = "Iconic building of Paris."
                    },
                },
            },
        };
    }

    public IEnumerable<CityDto> Cities { get; }
}
