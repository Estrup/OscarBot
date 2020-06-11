using System;
using System.Collections.Generic;

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
        public string TmdbId { get; set; }
        public string ImdbId { get; set; }
        public string Title { get; set; }
        public string Language { get; set; }
        public string Plot { get; set; }
        public int? Runtime { get; set; }
        public bool Watched { get; set; }
        public bool Watchlist { get; set; }
        public bool Monitor { get; set; }
        public DateTime? WatchedDate { get; set; }
        public ulong ServerId { get; set; }
        public string AddedBy { get; set; }
        public string AddedByUsername { get; set; }
        public DateTime AddedAt { get; set; }

        public virtual ICollection<EventMovie> EventMovies { get; set; }
        public virtual ICollection<ReleaseDate> ReleaseDates { get; set; }
        public virtual ICollection<Credit> Credits { get; set; }
    }
}
