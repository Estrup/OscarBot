using System;
using System.Collections.Generic;

namespace OscarBot.Models
{
    public partial class Movie
    {
        public Movie()
        {
            EventMovies = new HashSet<EventMovie>();
        }
        public string Id { get; set; }
        public string Title { get; set; }
        public string Plot { get; set; }
        public string Language { get; set; }
        public string Director { get; set; }
        public string Actors { get; set; }
        public string Runtime { get; set; }
        public bool Watched { get; set; }
        public DateTime? WatchedDate { get; set; }
        public string ServerId { get; set; }
        public bool Picked { get; set; }
        public string AddedBy { get; set; }
        public string AddedByUsername { get; set; }
        public DateTime AddedAt { get; set; }

        public virtual ICollection<EventMovie> EventMovies { get; set; }
    }
}
