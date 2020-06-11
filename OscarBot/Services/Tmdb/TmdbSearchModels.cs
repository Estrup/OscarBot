using System;
using System.Collections.Generic;

namespace OscarBot.Services.Tmdb
{
    public partial class TmdbSearchResult
    {
        public List<MovieResult> MovieResults { get; set; }
        public List<object> PersonResults { get; set; }
        public List<object> TvResults { get; set; }
        public List<object> TvEpisodeResults { get; set; }
        public List<object> TvSeasonResults { get; set; }
    }

    public partial class MovieResult
    {
        public long Id { get; set; }
        public bool Video { get; set; }
        public long VoteCount { get; set; }
        public double VoteAverage { get; set; }
        public string Title { get; set; }
        public DateTimeOffset ReleaseDate { get; set; }
        public string OriginalLanguage { get; set; }
        public string OriginalTitle { get; set; }
        public List<long> GenreIds { get; set; }
        public string BackdropPath { get; set; }
        public bool Adult { get; set; }
        public string Overview { get; set; }
        public string PosterPath { get; set; }
        public double Popularity { get; set; }
    }
}
