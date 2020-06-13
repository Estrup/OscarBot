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
    public class AddPick : BaseDiscordCmd
    {
        public string IdOrUrl { get; set; }
        public int EventId { get; set; }
    }

    public class AddPickHandler : MediatR.AsyncRequestHandler<AddPick>
    {
        private readonly TmdbService omdbService;
        private readonly IServiceProvider serviceProvider;

        public AddPickHandler(
            TmdbService tmdbService,
            IServiceProvider serviceProvider
            )
        {
            this.omdbService = tmdbService;
            this.serviceProvider = serviceProvider;
        }
        protected override async Task Handle(AddPick request, CancellationToken cancellationToken)
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

            var alreadyselected = mevent.EventMovies.Select(x => x.Movie);

            if (alreadyselected.Any(x => x.ImdbId  == result.ImdbId)) await Context.Channel.SendMessageAsync($"Movie already added to event..");

            Movie movie;
            if (await db.Movie.AsQueryable().AnyAsync(x => x.ImdbId == result.ImdbId ))
            {
                movie = await db.Movie.AsQueryable().SingleAsync(x => x.ImdbId == result.ImdbId);

            }
            else
            {
                movie = Movie.FromTmdbMovie(result, Context.User);
            }
            mevent.EventMovies.Add(new EventMovie { Movie = movie });
            await db.SaveChangesAsync();

            await Context.Channel.SendMessageAsync($"***{ result.Title }*** added to event");
        }
    }
}