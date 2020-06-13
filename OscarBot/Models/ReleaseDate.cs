using System;
namespace OscarBot.Models
{
    public class ReleaseDate
    {
        public Guid Id { get; set; }
        public Guid MovieId { get; set; }
        public ReleaseDateType Type { get; set; }
        public DateTimeOffset Date { get; set; }
        public string Note { get; set; }
        public string LocationCode { get; set; }
    }

    public enum ReleaseDateType
    {
        Premiere = 1,
        Theatrical_limited = 2,
        Theatrical = 3,
        Digital = 4,
        Physical = 5,
        Tv = 6
    }
}
