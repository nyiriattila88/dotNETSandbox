﻿namespace Movies.Contracts.Responses;

public record MoviesResponse
{
    public required IEnumerable<MovieResponse> Movies { get; init; } = Enumerable.Empty<MovieResponse>();
}
