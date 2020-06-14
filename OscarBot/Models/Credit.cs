using System;
namespace OscarBot.Models
{
    public class Credit
    {
        public Guid Id { get; set; }
        public Guid MovieId { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public int Order { get; set; }
    }
}
