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
    public class DeleteEvent : BaseDiscordCmd
    {
        public int EventId { get; set; }
    }

    public class DeleteEventHandler : MediatR.AsyncRequestHandler<DeleteEvent>
    {
        private readonly OmdbService omdbService;
        private readonly IServiceProvider serviceProvider;

        public DeleteEventHandler(
            OmdbService omdbService,
            IServiceProvider serviceProvider
            )
        {
            this.omdbService = omdbService;
            this.serviceProvider = serviceProvider;
        }
        protected override async Task Handle(DeleteEvent request, CancellationToken cancellationToken)
        {
            var Context = request.Context;
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetService<BotDbContext>();

            var mevent = await db.Event
                .Include(x => x.EventMovies)
                .AsQueryable().SingleOrDefaultAsync(x => x.Id == request.EventId);
            if (mevent == null)
            {
                await Context.Channel.SendMessageAsync($"Event not found...");
                return;
            }

            db.Event.Remove(mevent);

            await db.SaveChangesAsync();
            await Context.Channel.SendMessageAsync($"Event deleted...");
        }
    }
}