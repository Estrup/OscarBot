using System;
using System.Threading;
using System.Threading.Tasks;
using OscarBot.Services;
using Microsoft.Extensions.DependencyInjection;
using OscarBot.Models;
using Microsoft.EntityFrameworkCore;
using Discord;
using System.Linq;

namespace OscarBot.Commands
{
    public class GetEvent : BaseDiscordCmd
    {
        public int EventId { get; set; }
    }

    public class GetEventHandler : MediatR.AsyncRequestHandler<GetEvent>
    {
        private readonly IServiceProvider serviceProvider;
        private string[] consults = new string[] { "horosopes", "the oracles", "bill gates" };

        public GetEventHandler(
            IServiceProvider serviceProvider
            )
        {
            this.serviceProvider = serviceProvider;
        }
        protected override async Task Handle(GetEvent request, CancellationToken cancellationToken)
        {
            var Context = request.Context;
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetService<BotDbContext>();

            var mevent = await db.Event
                .Include(x => x.EventMovies)
                .ThenInclude(x => x.Movie)
                .SingleOrDefaultAsync(x => x.Id == request.EventId);
            if (mevent == null)
            {
                await Context.Channel.SendMessageAsync($"Event not found...");
                return;
            }

            var description = $"'!m randompicks { mevent.Id } <n>' to pick <n> random movies for the event";
            description = description + Environment.NewLine + $"'!m addpick { mevent.Id } <idOrUrl>' to add a specific movie";

            var eb = new EmbedBuilder() { Title = "Event: " + mevent.Title, Description = description, Color = Color.DarkBlue };
            eb.AddField("Id", mevent.Id.ToString(), true);
            eb.AddField("Start", mevent.EventStart.ToString("dd-MM-yyyy HH:mm"), true);
            if (mevent.EventEnd != null)
                eb.AddField("End", mevent.EventEnd.Value.ToString("dd-mm-yyyy HH:mm"), true);

            await Context.Channel.SendMessageAsync(embed: eb.Build());

            var list = mevent.EventMovies.Select(x => x.Movie);
            int i = 0;
            if (list.Count() == 0)
            {
                await Context.Channel.SendMessageAsync($"There is nothing here :(");
                return;
            }
            foreach (var pick in list)
            {
                await Context.Channel.SendMessageAsync($"Pick # { i + 1 } : **{ pick.Title }** https://www.imdb.com/title/{ pick.Id}");
                i++;
                await Task.Delay(1500);
            }
        }
    }
}