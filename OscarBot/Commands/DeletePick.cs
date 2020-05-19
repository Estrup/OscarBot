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
    public class DeletePick : BaseDiscordCmd
    {
        public string IdOrUrl { get; set; }
        public int EventId { get; set; }
    }

    public class DeletePickandler : MediatR.AsyncRequestHandler<DeletePick>
    {
        private readonly OmdbService omdbService;
        private readonly IServiceProvider serviceProvider;

        public DeletePickandler(
            OmdbService omdbService,
            IServiceProvider serviceProvider
            )
        {
            this.omdbService = omdbService;
            this.serviceProvider = serviceProvider;
        }
        protected override async Task Handle(DeletePick request, CancellationToken cancellationToken)
        {
            var idOrUrl = request.IdOrUrl;
            var Context = request.Context;

            var result = await omdbService.Get(idOrUrl);
            if (result == null)
            {
                await Context.Channel.SendMessageAsync($"No movie with that id was found...");
            }

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

            var em = mevent.EventMovies.SingleOrDefault(x => x.MovieId == result.imdbID);
            if (em != null)
            {
                mevent.EventMovies.Remove(em);
                await db.SaveChangesAsync();
            }

            await Context.Channel.SendMessageAsync($"***{ result.Title }*** removed from event");
        }
    }
}