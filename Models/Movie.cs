namespace MovieMatch.Api.Models
{
    public sealed class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Overview { get; set; }
        public string? Genre { get; set; }
        public int? Year { get; set; }
        public double? Rating { get; set; }
        public string? PosterUrl { get; set; }
    }
}

