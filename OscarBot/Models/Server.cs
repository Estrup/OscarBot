using System;
namespace OscarBot.Models
{
    public class Server
    {

        public string Id { get; set; }
        public string Name { get; set; }
        public string AnnouncementChannelId { get; set; }
        public string TimeZone { get; set; }
        public string AnnounementTriggerTime { get; set; }
        public int LastEventNo { get; set; }
    }
}
