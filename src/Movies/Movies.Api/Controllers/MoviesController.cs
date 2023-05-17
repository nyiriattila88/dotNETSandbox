using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mapping;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers;

[ApiController]
public class MoviesController : ControllerBase
{
    private readonly IMovieRepository _movieRepository;

    public MoviesController(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }

    [HttpPost(ApiEndpoints.Movies.Create)]
    public async Task<ActionResult<MovieResponse>> Create([FromBody] CreateMovieRequest request)
    {
        Movie movie = request.MapToMovie();
        await _movieRepository.CreateAsync(movie);

        MovieResponse movieResponse = movie.MapToMovieResponse();
        return Created($"api/movies/{movieResponse.Id}", movieResponse);
    }

    [HttpGet(ApiEndpoints.Movies.GetAll)]
    public async Task<ActionResult<MoviesResponse>> GetAll()
    {
        IEnumerable<Movie> movies = await _movieRepository.GetAllAsync();
        return Ok(movies.MapToMoviesResponse());
    }

    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task<ActionResult<MovieResponse>> Get([FromRoute] Guid id)
    {
        Movie? movie = await _movieRepository.GetByIdAsync(id);
        return movie is null ? NotFound() : Ok(movie.MapToMovieResponse());
    }
}
