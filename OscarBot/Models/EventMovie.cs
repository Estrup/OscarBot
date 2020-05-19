using System;
namespace OscarBot.Models
{
    public class EventMovie
    {
        public int EventId { get; set; }
        public string MovieId { get; set; }

        public virtual Movie Movie { get; set; }
        public virtual Event Event { get; set; }
    }
}
