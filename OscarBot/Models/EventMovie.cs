using System;
namespace OscarBot.Models
{
    public class EventMovie
    {
        public Guid EventId { get; set; }
        public Guid MovieId { get; set; }

        public virtual Movie Movie { get; set; }
        public virtual Event Event { get; set; }
    }
}
