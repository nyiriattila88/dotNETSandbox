using System.Text.RegularExpressions;

namespace Movies.Application.Models;
public partial record Movie
{
    public required Guid Id { get; init; }

    public required string Title { get; init; }

    public string Slug
    {
        get
        {
            string sluggedTitle = SlugRegex().Replace(Title, string.Empty)
                         .ToLower()
                         .Replace(" ", "-");

            return $"{sluggedTitle}-{YearOfRelease}";
        }
    }

    public required int YearOfRelease { get; init; }

    public required List<string> Genres { get; init; } = new();

    [GeneratedRegex("[^0-9A-Za-z _-]", RegexOptions.NonBacktracking, 5)]
    private static partial Regex SlugRegex();
}
