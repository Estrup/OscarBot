using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;

namespace OscarBot.Services.Tmdb
{
    public partial class TmdbSearchResult
    {
        [JsonProperty("movie_results")]
        public List<MovieResult> MovieResults { get; set; }

        [JsonProperty("person_results")]
        public List<object> PersonResults { get; set; }

        [JsonProperty("tv_results")]
        public List<object> TvResults { get; set; }

        [JsonProperty("tv_episode_results")]
        public List<object> TvEpisodeResults { get; set; }

        [JsonProperty("tv_season_results")]
        public List<object> TvSeasonResults { get; set; }
    }

    public partial class MovieResult
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("video")]
        public bool Video { get; set; }

        [JsonProperty("vote_count")]
        public long VoteCount { get; set; }

        [JsonProperty("vote_average")]
        public double VoteAverage { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("release_date")]
        public DateTimeOffset ReleaseDate { get; set; }

        [JsonProperty("original_language")]
        public string OriginalLanguage { get; set; }

        [JsonProperty("original_title")]
        public string OriginalTitle { get; set; }

        [JsonProperty("genre_ids")]
        public List<long> GenreIds { get; set; }

        [JsonProperty("backdrop_path")]
        public string BackdropPath { get; set; }

        [JsonProperty("adult")]
        public bool Adult { get; set; }

        [JsonProperty("overview")]
        public string Overview { get; set; }

        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }

        [JsonProperty("popularity")]
        public double Popularity { get; set; }
    }
}
