using CityInfo.API.Models;

namespace CityInfo.API;

/// <summary>
/// In memory data before EF is added
/// </summary>
public class CitiesDataStore
{

    public IEnumerable<CityDto> Cities { get; } = new List<CityDto>
    {
        new CityDto()
        {
            Id = 1,
            Name = "New York City",
            Description = "A city from the USA.",
            PointsOfInterest = new List<PointOfInterestDto>
            {
                new PointOfInterestDto
                {
                    Id = 1,
                    Name = "Central Park",
                    Description = "The most visited park in the US."
                },
                new PointOfInterestDto
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
            PointsOfInterest = new List<PointOfInterestDto>
            {
                new PointOfInterestDto
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
            PointsOfInterest = new List<PointOfInterestDto>
            {
                new PointOfInterestDto
                {
                    Id = 1,
                    Name = "Eiffel Tower",
                    Description = "Iconic building of Paris."
                },
            },
        }
    };
}

