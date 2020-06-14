using System;
using System.Collections.Generic;
using System.Linq;
using Discord.WebSocket;
using OscarBot.Services.Tmdb;

namespace OscarBot.Models
{
    public partial class Movie
    {
        public Movie()
        {
            EventMovies = new HashSet<EventMovie>();
            ReleaseDates = new HashSet<ReleaseDate>();
            Credits = new HashSet<Credit>();
        }
        public Guid Id { get; set; }
        public long TmdbId { get; set; }
        public string ImdbId { get; set; }
        public string Title { get; set; }
        public string Language { get; set; }
        public string Plot { get; set; }
        public int? Runtime { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public bool Watched { get; set; }
        public bool Watchlist { get; set; }
        public bool Monitor { get; set; }
        public DateTime? WatchedDate { get; set; }
        public string ServerId { get; set; }
        public string AddedBy { get; set; }
        public string AddedByUsername { get; set; }
        public DateTime AddedAt { get; set; }

        public virtual ICollection<EventMovie> EventMovies { get; set; }
        public virtual ICollection<ReleaseDate> ReleaseDates { get; set; }
        public virtual ICollection<Credit> Credits { get; set; }

        public static Movie FromTmdbMovie(TmdbMovie tmdbMovie, SocketUser user)
        {
            var movie = new Movie();
            movie.Id = Guid.NewGuid();
            movie.AddedAt = DateTime.UtcNow;
            movie.AddedBy = user.Id.ToString();
            movie.AddedByUsername = user.Username;
            movie.TmdbId = tmdbMovie.Id;
            movie.ImdbId = tmdbMovie.ImdbId;
            movie.Title = tmdbMovie.Title;
            movie.Language = tmdbMovie.OriginalLanguage;
            movie.Plot = tmdbMovie.Tagline;
            movie.Runtime = (int?)tmdbMovie.Runtime;
            movie.ReleaseDate = tmdbMovie.ReleaseDate;
            movie.ReleaseDates = tmdbMovie.ReleaseDates.Results.Where(x => x.Iso3166_1 == "US" || x.Iso3166_1 == "DK")
               .Select(x =>
               {
                   var releasedate = new ReleaseDate();
                   releasedate.LocationCode = x.Iso3166_1;
                   return x.ReleaseDates.Select(r => new ReleaseDate
                   {
                       LocationCode = x.Iso3166_1,
                       Date = r.ReleaseDateReleaseDate,
                       Note = r.Note,
                       Type = (ReleaseDateType)r.Type
                   });
               }).SelectMany(x => x).ToList();
            movie.Credits = tmdbMovie.Credits.Crew.Where(x => x.Job == "Director").Select(x => new Credit { Id = Guid.NewGuid(), Name = x.Name, Role = "Director", Order = 0 }).ToList();
            movie.Credits = movie.Credits.Concat(tmdbMovie.Credits.Cast.OrderByDescending(x => x.Order).Take(10).Select(x => new Credit { Id = Guid.NewGuid(), Name = x.Name, Role = "Actor", Order = x.Order })).ToList();
            return movie;
        }
    }
}
