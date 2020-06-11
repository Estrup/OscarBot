using System;
namespace OscarBot.Models
{
    public class Server
    {

        public ulong Id { get; set; }
        public string Name { get; set; }
        public ulong AnnouncementChannel { get; set; }
        public string TimeZone { get; set; }
        public string AnnounementTriggerTime { get; set; }
    }
}
