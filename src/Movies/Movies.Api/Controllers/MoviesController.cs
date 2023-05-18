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
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request)
    {
        Movie movie = request.MapToMovie();
        await _movieRepository.CreateAsync(movie);

        MovieResponse movieResponse = movie.MapToMovieResponse();
        return CreatedAtAction(nameof(Get), new { idOrSlug = movieResponse.Id }, movieResponse);
    }

    [HttpGet(ApiEndpoints.Movies.GetAll)]
    public async Task<IActionResult> GetAll()
    {
        IEnumerable<Movie> movies = await _movieRepository.GetAllAsync();
        return Ok(movies.MapToMoviesResponse());
    }

    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug)
    {
        Movie? movie = Guid.TryParse(idOrSlug, out var id)
            ? await _movieRepository.GetByIdAsync(id)
            : await _movieRepository.GetBySlugAsync(idOrSlug);

        return movie is not null ? Ok(movie.MapToMovieResponse()) : NotFound();
    }

    [HttpPut(ApiEndpoints.Movies.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request)
    {
        Movie movie = request.MapToMovie(id);
        bool isUpdated = await _movieRepository.UpdateAsync(movie);
        return isUpdated ? Ok(movie.MapToMovieResponse()) : NotFound();
    }

    [HttpDelete(ApiEndpoints.Movies.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        bool isDeleted = await _movieRepository.DeleteByIdAsync(id);
        return isDeleted ? Ok() : NotFound();
    }
}
