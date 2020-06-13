using System;
using System.Collections.Generic;

namespace OscarBot.Services.Tmdb
{
    public partial class TmdbMovie
    {
        public bool Adult { get; set; }
        public string BackdropPath { get; set; }
        public object BelongsToCollection { get; set; }
        public long Budget { get; set; }
        public List<Genre> Genres { get; set; }
        public string Homepage { get; set; }
        public long Id { get; set; }
        public string ImdbId { get; set; }
        public string OriginalLanguage { get; set; }
        public string OriginalTitle { get; set; }
        public string Overview { get; set; }
        public double Popularity { get; set; }
        public string PosterPath { get; set; }
        public List<ProductionCompany> ProductionCompanies { get; set; }
        public List<ProductionCountry> ProductionCountries { get; set; }
        public DateTimeOffset ReleaseDate { get; set; }
        public long Revenue { get; set; }
        public long Runtime { get; set; }
        public List<SpokenLanguage> SpokenLanguages { get; set; }
        public string Status { get; set; }
        public string Tagline { get; set; }
        public string Title { get; set; }
        public bool Video { get; set; }
        public double VoteAverage { get; set; }
        public long VoteCount { get; set; }
        public Credits Credits { get; set; }
        public ReleaseDates ReleaseDates { get; set; }
    }

    public partial class Credits
    {
        public List<Cast> Cast { get; set; }
        public List<Crew> Crew { get; set; }
    }

    public partial class Cast
    {
        public long CastId { get; set; }
        public string Character { get; set; }
        public string CreditId { get; set; }
        public long Gender { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public long Order { get; set; }
        public string ProfilePath { get; set; }
    }

    public partial class Crew
    {
        public string CreditId { get; set; }
        public string Department { get; set; }
        public long Gender { get; set; }
        public long Id { get; set; }
        public string Job { get; set; }
        public string Name { get; set; }
        public string ProfilePath { get; set; }
    }

    public partial class Genre
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    public partial class ProductionCompany
    {
        public long Id { get; set; }
        public string LogoPath { get; set; }
        public string Name { get; set; }
        public string OriginCountry { get; set; }
    }

    public partial class ProductionCountry
    {
        public string Iso3166_1 { get; set; }
        public string Name { get; set; }
    }

    public partial class ReleaseDates
    {
        public List<ReleaseDateResults> Results { get; set; }
    }

    public partial class ReleaseDateResults
    {
        public string Iso3166_1 { get; set; }
        public List<ReleaseDate> ReleaseDates { get; set; }
    }

    public partial class ReleaseDate
    {
        public string Certification { get; set; }
        public string Iso639_1 { get; set; }
        public string Note { get; set; }
        public DateTimeOffset ReleaseDateReleaseDate { get; set; }
        public int Type { get; set; }
    }

    public partial class SpokenLanguage
    {
        public string Iso639_1 { get; set; }
        public string Name { get; set; }
    }

}
