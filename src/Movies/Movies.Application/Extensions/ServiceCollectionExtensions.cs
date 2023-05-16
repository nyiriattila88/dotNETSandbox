using Microsoft.Extensions.DependencyInjection;
using Movies.Application.Repositories;

namespace Movies.Application.Extensions;

public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IMovieRepository, MovieRepository>();
        return services;
    }
}
