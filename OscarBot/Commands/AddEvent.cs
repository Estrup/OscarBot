using System;
using System.Threading;
using System.Threading.Tasks;
using OscarBot.Services;
using Microsoft.Extensions.DependencyInjection;
using OscarBot.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Discord;

namespace OscarBot.Commands
{
    public class AddEvent : BaseDiscordCmd
    {
        public string Title { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
    }

    public class AddEventHandler : MediatR.AsyncRequestHandler<AddEvent>
    {
        private readonly OmdbService omdbService;
        private readonly IServiceProvider serviceProvider;

        public AddEventHandler(
            OmdbService omdbService,
            IServiceProvider serviceProvider
            )
        {
            this.omdbService = omdbService;
            this.serviceProvider = serviceProvider;
        }
        protected override async Task Handle(AddEvent request, CancellationToken cancellationToken)
        {
            var Context = request.Context;
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetService<BotDbContext>();
            var mevent = new Event
            {
                Title = request.Title,
                EventEnd = request.End,
                EventStart = request.Start,
                AddedBy = Context.User.Id.ToString(),
                AddedByUsername = Context.User.Username
            };
            db.Event.Add(mevent);

            await db.SaveChangesAsync();
            var description = $"'!m randompicks { mevent.Id } <n>' to pick <n> random movies for the event";
            description = description + Environment.NewLine + $"'!m addpick { mevent.Id } <idOrUrl>' to add a specific movie";

            var eb = new EmbedBuilder() { Title = "Event created: " + mevent.Title, Description = description, Color = Color.DarkBlue };
            eb.AddField("Id", mevent.Id.ToString(), true);
            eb.AddField("Start", mevent.EventStart.ToString("dd-MM-yyyy HH:mm"), true);
            if (mevent.EventEnd != null)
                eb.AddField("End", mevent.EventEnd.Value.ToString("dd-mm-yyyy HH:mm"), true);
            await Context.Channel.SendMessageAsync(embed: eb.Build());
        }
    }
}