using System;
using System.Collections.Generic;

namespace OscarBot.Models
{
    public class Event
    {
        public Event()
        {
            EventMovies = new HashSet<EventMovie>();
        }
        public Guid Id { get; set; }
        public string ServerId { get; set; }
        public string Title { get; set; }
        public DateTime EventStart { get; set; }
        public DateTime? EventEnd { get; set; }
        public bool Completed { get; set; }

        public virtual ICollection<EventMovie> EventMovies { get; set; }
        public string AddedBy { get; internal set; }
        public string AddedByUsername { get; internal set; }
        public int No { get; internal set; }
    }
}
