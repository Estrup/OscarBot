using System;
using System.Threading;
using System.Threading.Tasks;
using OscarBot.Services;
using Microsoft.Extensions.DependencyInjection;
using OscarBot.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace OscarBot.Commands
{
    public class CompleteEvent : BaseDiscordCmd
    {
        public int EventId { get; set; }
    }

    public class CompleteEventHandler : MediatR.AsyncRequestHandler<CompleteEvent>
    {
        private readonly IServiceProvider serviceProvider;

        public CompleteEventHandler(
            IServiceProvider serviceProvider
            )
        {
            this.serviceProvider = serviceProvider;
        }
        protected override async Task Handle(CompleteEvent request, CancellationToken cancellationToken)
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

            mevent.Completed = true;
            foreach (var m in mevent.EventMovies.Select(x => x.Movie))
            {
                m.Watched = true;
                m.WatchedDate = mevent.EventStart;
            }

            await db.SaveChangesAsync();

            await Context.Channel.SendMessageAsync($"Event and movie marked as watched!");

        }
    }
}