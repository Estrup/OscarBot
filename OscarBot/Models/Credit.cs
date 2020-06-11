using System;
namespace OscarBot.Models
{
    public class Credit
    {

        public Guid MovieId { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string TmdbId { get; set; }
        public int Order { get; set; }
    }
}
